using System.Runtime.Versioning;

namespace UsbModule.Win32.Identifier;

/// <summary>
/// PNP 식별자 정의.
/// </summary>
[SupportedOSPlatform(GlobalDefinition.SupportForWindows)]
public static class PnpIdentifier
{
    /// <summary>
    /// USB.
    /// </summary>
    public const string Usb = "USB";

    /// <summary>
    ///  Personal Computer Memory Card International Association.
    /// </summary>
    public const string PCCard = "PCMCIA";

    /// <summary>
    /// SCSI.
    /// </summary>
    public const string SCSI = "SCSI";
}
