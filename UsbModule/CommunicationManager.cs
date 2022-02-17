using Microsoft.Win32;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using UsbModule.Win32;
using UsbModule.Win32.IO;

#pragma warning disable SA1129 // Do not use default value type constructor

namespace UsbModule;

/// <summary>
/// Communication manager.
/// </summary>
[SupportedOSPlatform("windows")]
public sealed class UsbCommunicationManager : IDisposable
{
    private const int Timeout = 1_000;

    // USB 1.1 WriteFile maximum block size is 4096
    // USB 1.1 ReadFile in block chunks of 64 bytes
    // USB 2.0 ReadFile in block chunks of 512 bytes
    private const int BufferSize = 1 << 12;

    /// <summary>
    /// 외부에서 new 키워드로 생성할 수 없도록 잠급니다.
    /// </summary>
    private UsbCommunicationManager()
    {
    }

    /// <summary>
    /// USB Interface Class ID.
    /// </summary>
    public Guid ClassId { get; private init; }

    /// <summary>
    /// 인터페이스 이름.
    /// </summary>
    public string? InterfaceName { get; private init; }

    /// <summary>
    /// 장치 ID.
    /// </summary>
    public string? ProductID { get; private init; }

    /// <summary>
    /// Handle.
    /// </summary>
    public IntPtr Handle { get; private set; }

    /// <summary>
    /// 입력한 인자정보를 이용해 USB 통신 환경을 구성합니다.
    /// </summary>
    /// <param name="interfaceClassGuid">USB Interface Class ID.</param>
    /// <param name="interfaceName">장치 이름.</param>
    /// <param name="productId">장치 ID.</param>
    /// <returns><see cref="UsbCommunicationManager"/>.</returns>
    public static UsbCommunicationManager? Open(
        Guid interfaceClassGuid, string? interfaceName, string? productId)
    {
        var handle = GetHandle(interfaceClassGuid, interfaceName, productId);

        if (handle == IntPtr.Zero)
        {
            return null;
        }

        return new UsbCommunicationManager
        {
            Handle = handle,
            ClassId = interfaceClassGuid,
            InterfaceName = interfaceName,
            ProductID = productId,
        };
    }

    /// <summary>
    /// 해당 식별자를 갖는 USB 장치 목록들을 반환합니다.
    /// </summary>
    /// <param name="interfaceClassGuid">장치 인터페이스 형식 ID.</param>
    /// <param name="flags">flags.</param>
    /// <returns>장치 목록 Dictionary.</returns>
    /// <exception cref="Win32Exception">장치 식별 실패 시.</exception>
    public static Dictionary<string, DeviceInfo> GetDevices(Guid interfaceClassGuid, SetupApi.Digcf flags)
    {
        var devices = new Dictionary<string, DeviceInfo>();

        IntPtr device;

        try
        {
            device = SetupApi.SetupDiGetClassDevs(ref interfaceClassGuid, null, IntPtr.Zero, (uint)flags);
        }
        catch
        {
            device = (IntPtr)FileIO.InvalidHandleValue;
        }

        if (device == (IntPtr)FileIO.InvalidHandleValue)
        {
            return devices;
        }

        var deviceIndex = default(uint);
        var deviceInterfaceData = new SetupApi.SpDeviceInterfaceData();

        try
        {
            while (SetupApi.SetupDiEnumDeviceInterfaces(device, 0, ref interfaceClassGuid, deviceIndex, ref deviceInterfaceData))
            {
                ++deviceIndex;

                SetupApi.SetupDiGetDeviceInterfaceDetail(
                    device, ref deviceInterfaceData, IntPtr.Zero, 0, out var size, IntPtr.Zero);

                if (size == 0 || size > Marshal.SizeOf(typeof(SetupApi.SpDeviceInterfaceDetailData)) - sizeof(uint))
                {
                    continue;
                }

                var interfaceDetail = new SetupApi.SpDeviceInterfaceDetailData();
                var devInfoData = new SetupApi.SpDevInfoData();

                var isSuccess = SetupApi.SetupDiGetDeviceInterfaceDetail(
                    device, ref deviceInterfaceData, ref interfaceDetail, size, out _, ref devInfoData);

                if (!isSuccess)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var path = interfaceDetail.DevicePath;
                var name = Win32Api.GetDeviceRegistryProperty(
                    device, ref devInfoData, SetupApi.Spdrp.LocationInformation);

                var desc = string.Empty;
                var port = string.Empty;

                if (path!.StartsWith("\\\\?\\", StringComparison.OrdinalIgnoreCase))
                {
                    var key = string.Concat("##?#", path.AsSpan(4));

                    try
                    {
                        using var regKey = Registry.LocalMachine.OpenSubKey(
                            $"SYSTEM\\CurrentControlSet\\Control\\DeviceClasses\\{interfaceClassGuid:B}");

                        using var subKey = regKey?.OpenSubKey(key);

                        if (subKey != null)
                        {
                            using var entryKey = subKey.OpenSubKey("#\\Device Parameters");

                            desc = entryKey?.GetValue("Port Description")?.ToString();
                            port = entryKey?.GetValue("Port Number")?.ToString();
                        }
                    }
                    catch
                    {
                        // do nothing
                    }
                }

                devices[name] = new DeviceInfo(path, desc, port);
            }

            SetupApi.SetupDiDestroyDeviceInfoList(device);
        }
        catch
        {
            // ignore
        }

        return devices;
    }

    /// <summary>
    /// buffer 데이터를 USB에 전달합니다.
    /// </summary>
    /// <param name="buffer">데이터 버퍼.</param>
    /// <returns>작업수행 결과.</returns>
    public bool Write(params byte[] buffer)
    {
        if (Handle == IntPtr.Zero)
        {
            return false;
        }

        FileIO.CancelIo(Handle);

        var bytes = new byte[buffer.Length];
        Array.Copy(buffer, 0, bytes, 0, buffer.Length);

        var wo = new ManualResetEvent(false);
        var ov = new NativeOverlapped
        {
#pragma warning disable S3869 // "SafeHandle.DangerousGetHandle" should not be called
            EventHandle = wo.GetSafeWaitHandle().DangerousGetHandle(),
#pragma warning restore S3869 // "SafeHandle.DangerousGetHandle" should not be called
        };

        if (!FileIO.WriteFile(Handle, bytes, (uint)bytes.Length, out _, ref ov) &&
            Marshal.GetLastWin32Error() == FileIO.ErrorIOPending)
        {
            wo.WaitOne(Timeout, false);
        }

        return FileIO.GetOverlappedResult(Handle, ref ov, out var size, true) && size > 0;
    }

    /// <summary>
    /// USB로부터 데이터를 전달받습니다.
    /// </summary>
    /// <returns>수신된 데이터.</returns>
    public byte[] Read()
    {
        if (Handle == IntPtr.Zero)
        {
            return Array.Empty<byte>();
        }

        var readBuffer = new byte[BufferSize];
        Array.Clear(readBuffer, 0, readBuffer.Length);

        var sg = new AutoResetEvent(false);
        var ov = new NativeOverlapped
        {
            OffsetLow = 0,
            OffsetHigh = 0,
#pragma warning disable S3869 // "SafeHandle.DangerousGetHandle" should not be called
            EventHandle = sg.GetSafeWaitHandle().DangerousGetHandle(),
#pragma warning restore S3869 // "SafeHandle.DangerousGetHandle" should not be called
        };

        if (!FileIO.ReadFile(Handle, readBuffer, BufferSize, out _, ref ov) &&
            Marshal.GetLastWin32Error() == FileIO.ErrorIOPending)
        {
            sg.WaitOne(Timeout, false);
        }

        FileIO.GetOverlappedResult(Handle, ref ov, out var readLength, false);

        var readData = new byte[readLength];
        Array.Copy(readBuffer, readData, readLength);

        return readData;
    }

    /// <summary>
    /// USB 연결을 닫습니다.
    /// </summary>
    public void Close()
    {
        if ((int)Handle > 0)
        {
            FileIO.CloseHandle(Handle);
            Handle = IntPtr.Zero;
        }
    }

    /// <summary>
    /// Dispose instance.
    /// </summary>
    public void Dispose()
    {
        Close();
    }

    private static IntPtr GetHandle(Guid interfaceClassGuid, string? interfaceName, string? productId)
    {
        if (string.IsNullOrWhiteSpace(interfaceName))
        {
            throw new ArgumentException("빈 값은 사용할 수 없습니다.", interfaceName);
        }

        if (string.IsNullOrWhiteSpace(productId))
        {
            throw new ArgumentException("빈 값은 사용할 수 없습니다.", productId);
        }

        // 네트워크 주소?
        if (interfaceName.StartsWith("\\", StringComparison.InvariantCultureIgnoreCase))
        {
            return IntPtr.Zero;
        }

        var devices = GetDevices(interfaceClassGuid, SetupApi.Digcf.DeviceInterface | SetupApi.Digcf.Present);
        var devicePath = devices
            .FirstOrDefault(device => device.Value.Path?.Contains(productId, StringComparison.OrdinalIgnoreCase) ?? false)
            .Key;

        if (string.IsNullOrEmpty(devicePath))
        {
            return IntPtr.Zero;
        }

        return FileIO.CreateFile(
            devicePath,
            FileIO.FileAccess.Write | FileIO.FileAccess.Read,
            FileIO.FileShareMode.Read,
            IntPtr.Zero,
            FileIO.FileCreationDisposition.OpenAlways,
            FileIO.FileAttributes.Normal | FileIO.FileAttributes.SequentialScan | FileIO.FileAttributes.Overlapped,
            IntPtr.Zero);
    }
}
