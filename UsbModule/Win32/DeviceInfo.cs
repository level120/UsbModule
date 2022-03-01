using System.Runtime.Versioning;

namespace UsbModule.Win32;

/// <summary>
/// Device information.
/// </summary>
[SupportedOSPlatform(GlobalDefinition.SupportForWindows)]
public record DeviceInfo
{
    /// <summary>
    /// Device Type.
    /// </summary>
    public string? DeviceType { get; init; }

    /// <summary>
    /// Device Instance ID.
    /// </summary>
    public string? DeviceInstanceId { get; init; }

    /// <summary>
    /// Device Unique ID.
    /// </summary>
    public string? DeviceUniqueId { get; init; }

    /// <summary>
    /// Device Desc.
    /// </summary>
    public string? DeviceDesc { get; init; }

    /// <summary>
    /// Path.
    /// </summary>
    public string? Path { get; init; }

    /// <summary>
    /// Port Description.
    /// </summary>
    public string? PortDescription { get; private set; }

    /// <summary>
    /// Port.
    /// </summary>
    public int? Port { get; private set; }

    /// <summary>
    /// PortName.
    /// </summary>
    public string? PortName { get; private set; }

    /// <summary>
    /// Service.
    /// </summary>
    public string? Service { get; init; }

    /// <summary>
    /// Update class info.
    /// </summary>
    /// <param name="port">port.</param>
    /// <param name="portName">port name.</param>
    /// <param name="portDescription">port description.</param>
    /// <returns>this.</returns>
    public DeviceInfo UpdateClassInfo(int? port, string? portName, string? portDescription)
    {
        Port = port;
        PortName = portName;
        PortDescription = portDescription;

        return this;
    }
}
