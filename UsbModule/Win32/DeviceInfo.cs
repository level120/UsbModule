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
            Description?.GetHashCode(StringComparison.Ordinal) ?? 0,
            Port?.GetHashCode(StringComparison.Ordinal) ?? 0);
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
               Description == other.Description &&
               Port == other.Port;
    }
}
