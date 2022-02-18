using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using UsbModule.Win32;
using UsbModule.Win32.IO;

#pragma warning disable CA1031 // Do not catch general exception types
#pragma warning disable S3869 // "SafeHandle.DangerousGetHandle" should not be called
#pragma warning disable SA1129 // Do not use default value type constructor

namespace UsbModule;

/// <summary>
/// Communication manager.
/// </summary>
/// <example>
/// <code>
/// // 장치 가져오기(Flag 조합 이용)
/// var devices = UsbCommunicationManager.GetDevices(
///     DeviceSetupClasses.UsbPrinter, SetupApi.Digcf.DeviceInterface | SetupApi.Digcf.AllClasses);
///
/// // 통신할 장치 선택
/// var device = devices.Values
///     .FirstOrDefault(device => device.Path?.Contains(id, StringComparison.OrdinalIgnoreCase) ?? false);
///
/// // 통신을 위한 매니저 생성
/// using var manager = UsbCommunicationManager.Open(device);
///
/// // 핸들 가져오기에 실패한 경우
/// if (manager == null)
/// {
///     throw new Win32Exception("Handle을 가져올 수 없습니다.");
/// }
///
/// // USB에 데이터 쓰기(byte[])
/// var isSuccess = manager.Write(command);
///
/// Console.WriteLine($"Write result : {isSuccess}");
///
/// // USB로부터 데이터 읽기(byte[])
/// var readString = manager.Read();
///
/// Console.WriteLine(Encoding.Default.GetString(readString));
/// </code>
/// </example>
[SupportedOSPlatform("windows")]
public sealed class UsbCommunicationManager : IDisposable
{
    private const int Timeout = 10_000;

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
    /// Handle.
    /// </summary>
    public SafeFileHandle? Handle { get; private set; }

    /// <summary>
    /// USB 통신 환경을 구성합니다.
    /// </summary>
    /// <param name="deviceInfo">Device info.</param>
    /// <returns><see cref="UsbCommunicationManager"/>.</returns>
    public static UsbCommunicationManager? Open(DeviceInfo deviceInfo)
    {
        var handle = GetHandle(deviceInfo);

        if (handle == null)
        {
            return null;
        }

        return new UsbCommunicationManager { Handle = handle };
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

                var portName = string.Empty;
                var desc = string.Empty;
                var port = 0;

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

                            var originBaseName = entryKey!.GetValue("Base Name")?.ToString();
                            desc = entryKey.GetValue("Port Description")!.ToString();
                            port = (int)entryKey.GetValue("Port Number")!;
                            portName = $"{originBaseName}{port:00#}";
                        }
                    }
                    catch
                    {
                        // do nothing
                    }
                }

                devices[name] = new DeviceInfo(path, portName, desc, port);
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
    /// <exception cref="Win32Exception">빈 데이터 사용시.</exception>
    public bool Write(params byte[] buffer)
    {
        var handle = Handle!.DangerousGetHandle();

        FileIO.CancelIo(handle);

        if (buffer == null || buffer.Length == 0)
        {
            throw new Win32Exception("빈 데이터를 입력할 수 없습니다.");
        }

        var bytes = new byte[buffer.Length];
        Array.Copy(buffer, 0, bytes, 0, buffer.Length);

        using var wo = new ManualResetEvent(false);
        var ov = new NativeOverlapped
        {
            EventHandle = wo.GetSafeWaitHandle().DangerousGetHandle(),
        };

        if (!FileIO.WriteFile(handle, bytes, (uint)bytes.Length, out _, ref ov) &&
            Marshal.GetLastWin32Error() == FileIO.ErrorIOPending)
        {
            wo.WaitOne(Timeout, false);
        }

        return FileIO.GetOverlappedResult(handle, ref ov, out var size, true) && size == buffer.LongLength;
    }

    /// <summary>
    /// USB로부터 데이터를 전달받습니다.
    /// </summary>
    /// <returns>수신된 데이터.</returns>
    public byte[] Read()
    {
        var handle = Handle!.DangerousGetHandle();

        var readBuffer = new byte[BufferSize];
        Array.Clear(readBuffer, 0, readBuffer.Length);

        using var sg = new AutoResetEvent(false);
        var ov = new NativeOverlapped
        {
            OffsetLow = 0,
            OffsetHigh = 0,
            EventHandle = sg.GetSafeWaitHandle().DangerousGetHandle(),
        };

        if (!FileIO.ReadFile(handle, readBuffer, BufferSize, out _, ref ov) &&
            Marshal.GetLastWin32Error() == FileIO.ErrorIOPending)
        {
            sg.WaitOne(Timeout, false);
        }

        FileIO.GetOverlappedResult(handle, ref ov, out var readLength, false);

        var readData = new byte[readLength];
        Array.Copy(readBuffer, readData, readLength);

        return readData;
    }

    /// <summary>
    /// USB 연결을 닫습니다.
    /// </summary>
    public void Close()
    {
        if (Handle?.IsClosed == false)
        {
            Handle.Dispose();
            Handle = null;
        }
    }

    /// <summary>
    /// Dispose instance.
    /// </summary>
    public void Dispose()
    {
        Close();
    }

    /// <summary>
    /// <see cref="DeviceInfo"/>를 이용해 Handle을 구성합니다.
    /// </summary>
    /// <param name="deviceInfo">Device 정보.</param>
    /// <returns>Handle.</returns>
    private static SafeFileHandle? GetHandle(DeviceInfo deviceInfo)
    {
        if (string.IsNullOrEmpty(deviceInfo.Path))
        {
            return null;
        }

        return FileIO.CreateFile(
            deviceInfo.Path,
            FileIO.FileAccess.Write | FileIO.FileAccess.Read,
            FileIO.FileShareMode.Read,
            IntPtr.Zero,
            FileIO.FileCreationDisposition.OpenAlways,
            FileIO.FileAttributes.Normal | FileIO.FileAttributes.SequentialScan | FileIO.FileAttributes.Overlapped,
            IntPtr.Zero);
    }
}
