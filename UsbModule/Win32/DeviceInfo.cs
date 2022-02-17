namespace UsbModule.Win32;

/// <summary>
/// Device information.
/// </summary>
public readonly struct DeviceInfo
{
    /// <summary>
    /// Initializes a new instance of the <see cref="DeviceInfo"/> struct.
    /// </summary>
    /// <param name="name">name.</param>
    /// <param name="value">value.</param>
    public DeviceInfo(string? name, string? value)
    {
        Name = name;
        Value = value;
    }

    /// <summary>
    /// Name.
    /// </summary>
    public string? Name { get; }

    /// <summary>
    /// Value.
    /// </summary>
    public string? Value { get; }
}
