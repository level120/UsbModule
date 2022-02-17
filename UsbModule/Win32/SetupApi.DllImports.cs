using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#pragma warning disable SA1600 // Elements should be documented

namespace UsbModule.Win32;

/// <summary>
/// Win32 SetupApi.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class SetupApi
{
    [DllImport(LibraryDll, CharSet = CharSet.Auto)]
    internal static extern IntPtr SetupDiGetClassDevs(
        ref Guid classGuid,
        [MarshalAs(UnmanagedType.LPWStr)]
        string? enumerator,
        IntPtr hwndParent,
        uint flags);

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern ushort SetupDiDestroyDeviceInfoList(IntPtr hDevInfo);

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiEnumDeviceInterfaces(
        IntPtr hDevInfo,
        int zeroDevInfo,
        ref Guid interfaceClassGuid,
        uint memberIndex,
        ref SpDeviceInterfaceData deviceInterfaceData);

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(
        IntPtr hDevInfo,
        ref SpDeviceInterfaceData deviceInterfaceData,
        ref SpDeviceInterfaceDetailData deviceInterfaceDetailData,
        uint deviceInterfaceDetailDataSize,
        out uint requiredSize,
        ref SpDevInfoData deviceInfoData);

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(
        IntPtr hDevInfo,
        ref SpDeviceInterfaceData deviceInterfaceData,
        IntPtr deviceInterfaceDetailData,
        uint deviceInterfaceDetailDataSize,
        out uint requiredSize,
        IntPtr deviceInfoData);

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceRegistryProperty(
        IntPtr hDevInfo,
        ref SpDevInfoData deviceInfoData,
        uint property,
        out IntPtr propertyRegDataType,
        char[]? propertyBuffer,
        uint propertyBufferSize,
        out uint requiredSize);
}
