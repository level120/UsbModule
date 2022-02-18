namespace UsbModule.Win32;

/// <summary>
/// Device information.
/// </summary>
public readonly struct DeviceInfo : IEquatable<DeviceInfo>
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceInfo"/> struct.
    /// </summary>
    /// <param name="path">path.</param>
    /// <param name="portName">base name.</param>
    /// <param name="description">description.</param>
    /// <param name="port">port.</param>
    public DeviceInfo(string? path, string? portName, string? description, int port)
    {
        Path = path;
        Description = description;
        Port = port;
        PortName = portName;
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
    public int Port { get; }

    /// <summary>
    /// PortName.
    /// </summary>
    public string? PortName { get; }

    /// <summary>
    /// Equals.
    /// </summary>
    /// <param name="left">left.</param>
    /// <param name="right">right.</param>
    /// <returns>equals.</returns>
    public static bool operator ==(DeviceInfo left, DeviceInfo right)
    {
        return left.Equals(right);
    }

    /// <summary>
    /// Not equals.
    /// </summary>
    /// <param name="left">left.</param>
    /// <param name="right">right.</param>
    /// <returns>Not equals.</returns>
    public static bool operator !=(DeviceInfo left, DeviceInfo right)
    {
        return !(left == right);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return HashCode.Combine(
            Path?.GetHashCode(StringComparison.Ordinal) ?? 0,
            PortName?.GetHashCode(StringComparison.Ordinal) ?? 0,
            Description?.GetHashCode(StringComparison.Ordinal) ?? 0,
            Port.GetHashCode());
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        return obj is DeviceInfo deviceInfo && Equals(deviceInfo);
    }

    /// <summary>
    /// Equals.
    /// </summary>
    /// <param name="other">other.</param>
    /// <returns>Equals.</returns>
    public bool Equals(DeviceInfo other)
    {
        return Path == other.Path &&
               PortName == other.PortName &&
               Description == other.Description &&
               Port == other.Port;
    }
}
