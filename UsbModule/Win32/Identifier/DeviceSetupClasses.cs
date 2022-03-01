using System.Runtime.Versioning;

namespace UsbModule.Win32.Identifier;

/// <summary>
/// Windows에서 제공하는 식별자.
/// </summary>
[SupportedOSPlatform(GlobalDefinition.SupportForWindows)]
public static class DeviceSetupClasses
{
    /// <summary>
    /// USB Printer의 식별자<br/>
    /// Note: usbprint.sys.
    /// </summary>
    public static readonly Guid UsbPrinter =
        new(0x28d78fad, 0x5a12, 0x11D1, 0xae, 0x5b, 0x00, 0x00, 0xf8, 0x03, 0xa8, 0xc2);
}
