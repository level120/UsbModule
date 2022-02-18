using System.Runtime.Versioning;

// Enum warning
#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
#pragma warning disable CA1028 // Enum Storage should be Int32
#pragma warning disable CA1008 // Enums should have zero value

#pragma warning disable RCS1237 // Use bit shift operator.
#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

namespace UsbModule.Win32.IO;

/// <summary>
/// 구조체 정의.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class FileIO
{
    /// <summary>
    /// from winnt.h.
    /// </summary>
    [Flags]
    public enum FileAccess : uint
    {
        All = 0x10000000,
        Execute = 0x20000000,
        Write = 0x40000000,
        Read = 0x80000000,
    }

    /// <summary>
    /// from winnt.h.
    /// </summary>
    [Flags]
    public enum FileShareMode : uint
    {
        Read = 0x00000001,
        Write = 0x00000002,
        Delete = 0x00000004,
    }

    /// <summary>
    /// from winbase.h.
    /// </summary>
    public enum FileCreationDisposition : uint
    {
        CreateNew = 1,
        CreateAlways = 2,
        OpenExisting = 3,
        OpenAlways = 4,
        TurncateExisting = 5,
    }

    /// <summary>
    /// from winnt.h.
    /// </summary>
    [Flags]
    public enum FileAttributes : uint
    {
        Readonly = 0x00000001,
        Hidden = 0x00000002,
        System = 0x00000004,
        Directory = 0x00000010,
        Archive = 0x00000020,
        Device = 0x00000040,
        Normal = 0x00000080,
        Temporary = 0x00000100,
        SparseFile = 0x00000200,
        ReparsePoint = 0x00000400,
        Compressed = 0x00000800,
        Offline = 0x00001000,
        NotContentIndexed = 0x00002000,
        Encrypted = 0x00004000,
        FirstPipeInstance = 0x00080000,
        OpenNoRecall = 0x00100000,
        OpenReparsePoint = 0x00200000,
        PosixSemantics = 0x01000000,
        BackupSemantics = 0x02000000,
        DeleteOnClose = 0x04000000,
        SequentialScan = 0x08000000,
        RandomAccess = 0x10000000,
        NoBuffering = 0x20000000,
        Overlapped = 0x40000000,
        WriteThrough = 0x80000000,
    }
}
