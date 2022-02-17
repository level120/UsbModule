namespace UsbModule.Win32;

/// <summary>
/// Device information.
/// </summary>
public readonly struct DeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceInfo"/> struct.
    /// </summary>
    /// <param name="path">path.</param>
    /// <param name="description">description.</param>
    /// <param name="port">port.</param>
    public DeviceInfo(string? path, string? description, string? port)
    {
        Path = path;
        Description = description;
        Port = port;
    }

    /// <summary>
    /// Path.
    /// </summary>
    public string? Path { get; }

    /// <summary>
    /// Description.
    /// </summary>
    public string? Description { get; }

    /// <summary>
    /// Port.
    /// </summary>
    public string? Port { get; }
}
