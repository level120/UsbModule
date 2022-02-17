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
       ref SpDevInfoData devInfo,
       ref Guid interfaceClassGuid,
       uint memberIndex,
       ref SpDeviceInterfaceData deviceInterfaceData);

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiEnumDeviceInterfaces(
        IntPtr hDevInfo,
        int zeroDevInfo,  // used for 0 (Zero)
        ref Guid interfaceClassGuid,
        uint memberIndex,
        ref SpDeviceInterfaceData deviceInterfaceData);

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(
        IntPtr hDevInfo,
        ref SpDeviceInterfaceData deviceInterfaceData,
        ref SpDeviceInterfaceDetailData deviceInterfaceDetailData,  // OPTIONAL
        uint deviceInterfaceDetailDataSize,
        [Out]/* out uint */ NullClass? requiredSize,  // OPTIONAL
        ref SpDevInfoData deviceInfoData);  // OPTIONAL

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(
        IntPtr hDevInfo,
        ref SpDeviceInterfaceData deviceInterfaceData,
        NullClass? deviceInterfaceDetailData,  // OPTIONAL
        uint deviceInterfaceDetailDataSize,
        out uint requiredSize,  // OPTIONAL
        NullClass? deviceInfoData);  // OPTIONAL

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceInterfaceDetail(
       IntPtr hDevInfo,
       ref SpDeviceInterfaceData deviceInterfaceData,
       int zeroDeviceInterfaceDetailData,  // used for 0 (Zero)
       uint zeroDeviceInterfaceDetailDataSize,
       out uint requiredSize,
       int zeroDeviceInfoData);  // used for 0 (Zero)  // KEEP

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceRegistryProperty(
        IntPtr hDevInfo,
        ref SpDevInfoData deviceInfoData,
        uint property,
        [Out]/* out IntPtr */ NullClass? propertyRegDataType,  // OPTIONAL
        [Out] char[] propertyBuffer,
        uint propertyBufferSize,
        [Out]/* out IntPtr */ NullClass? requiredSize);  // OPTIONAL

    [DllImport(LibraryDll, CharSet = CharSet.Auto, SetLastError = true)]
    internal static extern bool SetupDiGetDeviceRegistryProperty(
        IntPtr hDevInfo,
        ref SpDevInfoData deviceInfoData,
        uint property,
        out IntPtr propertyRegDataType,  // OPTIONAL
        NullClass? propertyBuffer,
        uint propertyBufferSize,  // set to 0 (Zero)
        out uint requiredSize);  // OPTIONAL
}
