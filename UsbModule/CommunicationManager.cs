using Microsoft.Win32;
using PInvoke;
using System.Runtime.Versioning;

#pragma warning disable S3869 // "SafeHandle.DangerousGetHandle" should not be called

namespace UsbModule;

/// <summary>
/// Communication manager.
/// </summary>
/// <example>
/// <code>
/// // 장치 가져오기(Flag 조합 이용)
/// var devices = UsbCommunicationManager.GetDevices(
///     DeviceSetupClasses.UsbPrinter, SetupApi.GetClassDevsFlags.DIGCF_DEVICEINTERFACE | SetupApi.GetClassDevsFlags.DIGCF_ALLCLASSES);
///
/// // 통신할 장치 선택
/// var device = devices
///     .FirstOrDefault(device => device.DeviceInstanceId?.Contains(id, StringComparison.OrdinalIgnoreCase) ?? false);
///
/// // 통신을 위한 매니저 생성
/// using var manager = UsbCommunicationManager.Open(device);
///
/// // 핸들 가져오기에 실패한 경우
/// if (manager.IsInvalid)
/// {
///     throw new InvalidDataException("Invalid handle.");
/// }
///
/// // USB에 데이터 쓰기(byte[])
/// var isSuccess = manager.Write(0x1d, 0x49, 0x02);
///
/// Console.WriteLine($"Write result : {isSuccess}");
///
/// // USB로부터 데이터 읽기(byte[])
/// var readString = manager.Read();
///
/// Console.WriteLine(Encoding.Default.GetString(readString));
/// </code>
/// </example>
[SupportedOSPlatform(GlobalDefinition.SupportForWindows)]
public sealed class UsbCommunicationManager : IDisposable
{
    // USB 1.1 WriteFile maximum block size is 4096
    // USB 1.1 ReadFile in block chunks of 64 bytes
    // USB 2.0 ReadFile in block chunks of 512 bytes
    private const int BufferSize = 1 << 12;

    private bool _disposed;

    /// <summary>
    /// 외부에서 new 키워드로 생성할 수 없도록 잠급니다.
    /// </summary>
    private UsbCommunicationManager()
    {
        Handle = Kernel32.SafeObjectHandle.Null;
    }

    /// <summary>
    /// Handle.
    /// </summary>
    public Kernel32.SafeObjectHandle Handle { get; private init; }

    /// <summary>
    /// When invalid communication.
    /// </summary>
    public bool IsInvalid => Handle.IsInvalid;

    /// <summary>
    /// USB 통신 환경을 구성합니다.
    /// </summary>
    /// <param name="deviceInfo">Device info.</param>
    /// <returns><see cref="UsbCommunicationManager"/>.</returns>
    public static UsbCommunicationManager Open(DeviceInfo? deviceInfo)
    {
        return new UsbCommunicationManager
        {
            Handle = GetHandle(deviceInfo),
        };
    }

    /// <summary>
    /// 해당 식별자를 갖는 USB 장치 목록들을 반환합니다.
    /// </summary>
    /// <param name="interfaceClassGuid">장치 인터페이스 형식 ID.</param>
    /// <param name="flags">flags.</param>
    /// <returns>장치 목록 Dictionary.</returns>
    /// <exception cref="Win32Exception">장치 식별 실패 시.</exception>
    public static unsafe IReadOnlyCollection<DeviceInfo> GetDevices(
        Guid interfaceClassGuid, SetupApi.GetClassDevsFlags flags)
    {
        var devices = new List<DeviceInfo>();

        using var device = SetupApi.SetupDiGetClassDevs(&interfaceClassGuid, null, IntPtr.Zero, flags);

        if (device == null || device == SetupApi.SafeDeviceInfoSetHandle.Invalid)
        {
            return devices;
        }

        var deviceIndex = 0;
        var deviceInterfaceData = SetupApi.SP_DEVICE_INTERFACE_DATA.Create();

        while (SetupApi.SetupDiEnumDeviceInterfaces(
                   device, IntPtr.Zero, ref interfaceClassGuid, deviceIndex++, ref deviceInterfaceData))
        {
            int size;

            SetupApi.SetupDiGetDeviceInterfaceDetail(device, ref deviceInterfaceData, null, 0, &size, null);

            var devInfoData = SetupApi.SP_DEVINFO_DATA.Create();
            var path = SetupApi.SetupDiGetDeviceInterfaceDetail(device, deviceInterfaceData, &devInfoData);
            var deviceInfo = GetDeviceInfoWithDeviceClass(path, interfaceClassGuid);

            if (deviceInfo != null)
            {
                devices.Add(deviceInfo);
            }
        }

        return devices.AsReadOnly();
    }

    /// <summary>
    /// buffer 데이터를 USB에 전달합니다.
    /// </summary>
    /// <param name="buffer">데이터 버퍼.</param>
    /// <returns>작업수행 결과.</returns>
    /// <exception cref="Win32Exception">IO Cancel 실패 시.</exception>
    public unsafe bool Write(params byte[] buffer)
    {
        if (!Kernel32.CancelIo(Handle))
        {
            throw new Win32Exception(Kernel32.GetLastError());
        }

        fixed (byte* pBuffer = buffer)
        {
            using var waitEvent = new ManualResetEvent(false);
            var overlapped = new NativeOverlapped
            {
                EventHandle = waitEvent.SafeWaitHandle.DangerousGetHandle(),
            };

            int writeCount;
            var isSuccess = Kernel32.WriteFile(Handle, pBuffer, buffer.Length, &writeCount, &overlapped);

            if (!isSuccess && Kernel32.GetLastError() == Win32ErrorCode.ERROR_IO_PENDING)
            {
                waitEvent.WaitOne();
            }

            return Kernel32.GetOverlappedResult(Handle, &overlapped, out var transferred, bWait: true) &&
                   transferred == buffer.LongLength;
        }
    }

    /// <summary>
    /// USB로부터 데이터를 전달받습니다.
    /// </summary>
    /// <returns>수신된 데이터.</returns>
    public unsafe ArraySegment<byte> Read()
    {
        var buffer = new byte[BufferSize];

        fixed (byte* pBuffer = buffer)
        {
            using var waitEvent = new AutoResetEvent(false);
            var overlapped = new NativeOverlapped
            {
                OffsetLow = 0,
                OffsetHigh = 0,
                EventHandle = waitEvent.SafeWaitHandle.DangerousGetHandle(),
            };

            var isSuccess = Kernel32.ReadFile(Handle, pBuffer, buffer.Length, null, &overlapped);

            if (!isSuccess && Kernel32.GetLastError() == Win32ErrorCode.ERROR_IO_PENDING)
            {
                waitEvent.WaitOne();
            }

            Kernel32.GetOverlappedResult(Handle, &overlapped, out var transferred, bWait: true);

            return new ArraySegment<byte>(buffer[..transferred]);
        }
    }

    /// <summary>
    /// USB 연결을 닫습니다.
    /// </summary>
    public void Close()
    {
        Dispose();
    }

    /// <summary>
    /// Dispose instance.
    /// </summary>
    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (!Handle.IsClosed)
        {
            Handle.Dispose();
        }
    }

    /// <summary>
    /// <see cref="DeviceInfo"/>를 이용해 Handle을 구성합니다.
    /// </summary>
    /// <param name="deviceInfo">Device 정보.</param>
    /// <returns>Handle.</returns>
    private static Kernel32.SafeObjectHandle GetHandle(DeviceInfo? deviceInfo)
    {
        if (string.IsNullOrEmpty(deviceInfo?.Path))
        {
            return Kernel32.SafeObjectHandle.Invalid;
        }

        return Kernel32.CreateFile(
            deviceInfo.Path,
            Kernel32.FileAccess.FILE_GENERIC_READ | Kernel32.FileAccess.FILE_GENERIC_WRITE,
            Kernel32.FileShare.FILE_SHARE_READ,
            IntPtr.Zero,
            Kernel32.CreationDisposition.OPEN_ALWAYS,
            Kernel32.CreateFileFlags.FILE_ATTRIBUTE_NORMAL | Kernel32.CreateFileFlags.FILE_FLAG_SEQUENTIAL_SCAN | Kernel32.CreateFileFlags.FILE_FLAG_OVERLAPPED,
            Kernel32.SafeObjectHandle.Null);
    }

    private static DeviceInfo? GetDeviceInfo(string devicePath)
    {
        var pathItem = devicePath.Split('#');

        if (pathItem.Length < 4)
        {
            return null;
        }

        var deviceType = pathItem[0][4..];
        var deviceInstanceId = pathItem[1];
        var deviceUniqueId = pathItem[2];

        try
        {
            using var regKey = Registry.LocalMachine.OpenSubKey(
                @$"SYSTEM\CurrentControlSet\Enum\{deviceType}\{deviceInstanceId}\{deviceUniqueId}");

            var service = regKey?.GetValue("Service") as string;
            var description = regKey?.GetValue("DeviceDesc") as string;

            return new DeviceInfo
            {
                DeviceType = deviceType,
                DeviceInstanceId = deviceInstanceId,
                DeviceUniqueId = deviceUniqueId,
                DeviceDesc = description,
                Service = service,
                Path = devicePath,
            };
        }
        catch
        {
            // do nothing
        }

        return null;
    }

    private static DeviceInfo? GetDeviceInfoWithDeviceClass(string devicePath, Guid deviceClassId)
    {
        var deviceInfo = GetDeviceInfo(devicePath);

        if (deviceInfo == null)
        {
            return null;
        }

        var deviceClassesKey = string.Concat("##?#", devicePath.AsSpan(4));

        try
        {
            using var regKey = Registry.LocalMachine.OpenSubKey(
                @$"SYSTEM\CurrentControlSet\Control\DeviceClasses\{deviceClassId:B}");

            using var subKey = regKey?.OpenSubKey(deviceClassesKey);
            using var entryKey = subKey?.OpenSubKey(@"#\Device Parameters");

            var originBaseName = entryKey?.GetValue("Base Name") as string;
            var description = entryKey?.GetValue("Port Description") as string;
            var port = entryKey?.GetValue("Port Number") as int?;
            var portName = $"{originBaseName}{port:00#}";

            return deviceInfo.UpdateClassInfo(port, portName, description);
        }
        catch
        {
            // do nothing
        }

        return null;
    }
}
