using Microsoft.Win32;
using PInvoke;
using System.Collections.ObjectModel;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;
using UsbModule.Win32;

#pragma warning disable S1481 // Unused local variables should be removed

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
    public Kernel32.SafeObjectHandle? Handle { get; private set; }

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
    public static unsafe IReadOnlyDictionary<string, DeviceInfo> GetDevices(
        Guid interfaceClassGuid, SetupApi.GetClassDevsFlags flags)
    {
        var devices = new Dictionary<string, DeviceInfo>();

        SetupApi.SafeDeviceInfoSetHandle device;

        try
        {
            device = SetupApi.SetupDiGetClassDevs(&interfaceClassGuid, null, IntPtr.Zero, flags);
        }
        catch
        {
            device = SetupApi.SafeDeviceInfoSetHandle.Invalid;
        }

        if (device == SetupApi.SafeDeviceInfoSetHandle.Invalid)
        {
            return devices;
        }

        var deviceIndex = default(int);
        var deviceInterfaceData = SetupApi.SP_DEVICE_INTERFACE_DATA.Create();

        try
        {
            while (SetupApi.SetupDiEnumDeviceInterfaces(
                       device, IntPtr.Zero, ref interfaceClassGuid, deviceIndex, ref deviceInterfaceData))
            {
                ++deviceIndex;

                int size;

                SetupApi.SetupDiGetDeviceInterfaceDetail(device, ref deviceInterfaceData, null, 0, &size, null);

                if (size == 0)
                {
                    continue;
                }

                var interfaceDetail = default(SetupApi.SP_DEVICE_INTERFACE_DETAIL_DATA);
                var devInfoData = SetupApi.SP_DEVINFO_DATA.Create();

                var isSuccess = SetupApi.SetupDiGetDeviceInterfaceDetail(
                    device, ref deviceInterfaceData, &interfaceDetail, size, null, &devInfoData);

                if (!isSuccess)
                {
                    throw new Win32Exception(Marshal.GetLastWin32Error());
                }

                var name3 = interfaceClassGuid;
                var name2 = devInfoData.ClassGuid;
                var name = deviceInterfaceData.InterfaceClassGuid;
                var path = interfaceDetail.DevicePath->ToString();
                var portName = string.Empty;
                var desc = string.Empty;
                var port = 0;

                if (path.StartsWith("\\\\?\\", StringComparison.OrdinalIgnoreCase))
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

                devices[name.ToString()] = new DeviceInfo(path, portName, desc, port);
            }
        }
        catch
        {
            // ignore
        }
        finally
        {
            device.Dispose();
        }

        return new ReadOnlyDictionary<string, DeviceInfo>(devices);
    }

    /// <summary>
    /// buffer 데이터를 USB에 전달합니다.
    /// </summary>
    /// <param name="buffer">데이터 버퍼.</param>
    /// <returns>작업수행 결과.</returns>
    /// <exception cref="Win32Exception">빈 데이터 사용시.</exception>
    public bool Write(ArraySegment<byte> buffer)
    {
        Kernel32.CancelIo(Handle);

        if (buffer.Count == 0)
        {
            throw new Win32Exception();
        }

        return Kernel32.WriteFile(Handle, buffer) == buffer.Count;
    }

    /// <summary>
    /// USB로부터 데이터를 전달받습니다.
    /// </summary>
    /// <returns>수신된 데이터.</returns>
    public ArraySegment<byte> Read()
    {
        var readData = new ArraySegment<byte>(new byte[BufferSize]);
        var readLength = Kernel32.ReadFile(Handle, readData);

        return readData.Slice(0, readLength);
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
    private static Kernel32.SafeObjectHandle? GetHandle(DeviceInfo deviceInfo)
    {
        if (string.IsNullOrEmpty(deviceInfo.Path))
        {
            return null;
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
}
