using System.Runtime.Versioning;

#pragma warning disable CA1034 // Nested types should not be visible
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

namespace UsbModule.Win32;

/// <summary>
/// 상수 정의.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class SetupApi
{
    /// <summary>
    /// SPDRP.
    /// </summary>
    public static class Spdrp
    {
        /// <summary>
        /// DeviceDesc (R/W).
        /// </summary>
        public const uint DeviceDesc = 0x00000000;

        /// <summary>
        /// HardwareID (R/W).
        /// </summary>
        public const uint HardwareId = 0x00000001;

        /// <summary>
        /// CompatibleIDs (R/W).
        /// </summary>
        public const uint CompatibleIds = 0x00000002;

        /// <summary>
        /// Unused.
        /// </summary>
        public const uint Unuse0 = 0x00000003;

        /// <summary>
        /// Service (R/W).
        /// </summary>
        public const uint Service = 0x00000004;

        /// <summary>
        /// Unused.
        /// </summary>
        public const uint Unuse1 = 0x00000005;

        /// <summary>
        /// Unused.
        /// </summary>
        public const uint Unuse2 = 0x00000006;

        /// <summary>
        /// Class (R--tied to ClassGUID).
        /// </summary>
        public const uint Class = 0x00000007;

        /// <summary>
        /// ClassGUID (R/W).
        /// </summary>
        public const uint ClassGuid = 0x00000008;

        /// <summary>
        /// Driver (R/W).
        /// </summary>
        public const uint Driver = 0x00000009;

        /// <summary>
        /// ConfigFlags (R/W).
        /// </summary>
        public const uint ConfigFlags = 0x0000000A;

        /// <summary>
        /// Mfg (R/W).
        /// </summary>
        public const uint Mfg = 0x0000000B;

        /// <summary>
        /// FriendlyName (R/W).
        /// </summary>
        public const uint FriendlyName = 0x0000000C;

        /// <summary>
        /// LocationInformation (R/W).
        /// </summary>
        public const uint LocationInformation = 0x0000000D;

        /// <summary>
        /// PhysicalDeviceObjectName (R).
        /// </summary>
        public const uint PhysicalDeviceObjectName = 0x0000000E;

        /// <summary>
        /// Capabilities (R).
        /// </summary>
        public const uint Capabilities = 0x0000000F;

        /// <summary>
        /// UiNumber (R).
        /// </summary>
        public const uint UINumber = 0x00000010;

        /// <summary>
        /// UpperFilters (R/W).
        /// </summary>
        public const uint UpperFilters = 0x00000011;

        /// <summary>
        /// LowerFilters (R/W).
        /// </summary>
        public const uint LowerFilters = 0x00000012;

        /// <summary>
        /// BusTypeGUID (R).
        /// </summary>
        public const uint BusTypeGuid = 0x00000013;

        /// <summary>
        /// LegacyBusType (R).
        /// </summary>
        public const uint LegacyBusType = 0x00000014;

        /// <summary>
        /// BusNumber (R).
        /// </summary>
        public const uint BusNumber = 0x00000015;

        /// <summary>
        /// EnumeratorName.
        /// </summary>
        public const uint EnumeratorName = 0x00000016;

        /// <summary>
        /// Enumerator Name (R).
        /// </summary>
        public const uint Security = 0x00000017;

        /// <summary>
        /// Security (R/W, binary form).
        /// </summary>
        public const uint SecuritySDS = 0x00000018;

        /// <summary>
        /// Security (W, SDS form).
        /// </summary>
        public const uint DevType = 0x00000019;

        /// <summary>
        /// Device Type (R/W).
        /// </summary>
        public const uint Exclusive = 0x0000001A;

        /// <summary>
        /// Device is exclusive-access (R/W).
        /// </summary>
        public const uint Characteristics = 0x0000001B;

        /// <summary>
        /// Device Characteristics (R/W).
        /// </summary>
        public const uint Address = 0x0000001C;

        /// <summary>
        /// Device Address (R).
        /// </summary>
        public const uint UINumberDescFormat = 0x0000001D;

        /// <summary>
        /// UiNumberDescFormat (R/W).
        /// </summary>
        public const uint DevicePowerData = 0x0000001E;

        /// <summary>
        /// Device Power Data (R).
        /// </summary>
        public const uint RemovalPolicy = 0x0000001F;

        /// <summary>
        /// Removal Policy (R).
        /// </summary>
        public const uint RemovalPolicyHWDefault = 0x00000020;

        /// <summary>
        /// Hardware Removal Policy (R).
        /// </summary>
        public const uint RemovalPolicyOverride = 0x00000021;

        /// <summary>
        /// Removal Policy Override (RW).
        /// </summary>
        public const uint InstallState = 0x00000022;

        /// <summary>
        /// Device Install State (R).
        /// </summary>
        public const uint MaximumProperty = 0x00000023;
    }
}
