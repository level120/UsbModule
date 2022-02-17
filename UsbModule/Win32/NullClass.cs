using System.Runtime.Versioning;

namespace UsbModule.Win32;

/// <summary>
/// Null, This is Error.
/// </summary>
[SupportedOSPlatform("windows")]
public class NullClass
{
    /// <summary>
    /// Initializes a new instance of the <see cref="NullClass"/> class.
    /// </summary>
    public NullClass()
    {
#pragma warning disable S112 // General exceptions should never be thrown
#pragma warning disable CA2201 // Do not raise reserved exception types
        throw new NullReferenceException("Cannot create instance of NullClass");
#pragma warning restore CA2201 // Do not raise reserved exception types
#pragma warning restore S112 // General exceptions should never be thrown
    }
}
