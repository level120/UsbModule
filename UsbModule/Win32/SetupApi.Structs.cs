using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace UsbModule.Win32;

/// <summary>
/// 구조체 정의.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class SetupApi
{
    /// <summary>
    /// SP_DEVINFO_DATA.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SpDevInfoData
    {
        /// <summary>
        /// Size of structure in bytes.
        /// </summary>
        public int CbSize;

        /// <summary>
        /// GUID of the device interface class.
        /// </summary>
        public Guid ClassGuid;

        /// <summary>
        /// Handle to this device instance.
        /// </summary>
        public int DevInst;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        public UIntPtr Reserved;
    }

    /// <summary>
    /// SP_DEVICE_INTERFACE_DATA.
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct SpDeviceInterfaceData
    {
        /// <summary>
        /// Size of the structure, in bytes.
        /// </summary>
        public int CbSize;

        /// <summary>
        /// GUID of the device interface class.
        /// </summary>
        public Guid InterfaceClassGuid;

        /// <summary>
        /// Flags.
        /// </summary>
        public uint Flags;

        /// <summary>
        /// Reserved; do not use.
        /// </summary>
        public UIntPtr Reserved;
    }

    /// <summary>
    /// SP_DEVICE_INTERFACE_DETAIL_DATA.
    /// </summary>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1)]
    public struct SpDeviceInterfaceDetailData
    {
        /// <summary>
        /// Size of the structure, in bytes.
        /// </summary>
        public uint CbSize;

        /// <summary>
        ///  Note: Will never be more than 265 in length.
        /// </summary>
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 256)]
        public string DevicePath;
    }
}
