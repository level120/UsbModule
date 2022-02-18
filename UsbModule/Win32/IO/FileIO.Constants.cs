using System.Runtime.Versioning;

#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
#pragma warning disable CA1724 // Type names should not match namespaces

namespace UsbModule.Win32.IO;

/// <summary>
/// 상수 정의.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class FileIO
{
    /// <summary>INVALID_HANDLE_VALUE.</summary>
    public const int InvalidHandleValue = -1;

    /// <summary>ERROR_FILE_NOT_FOUND.</summary>
    public const int ErrorFileNotFound = 2;

    /// <summary>ERROR_INVALID_NAME.</summary>
    public const int ErrorInvalidName = 123;

    /// <summary>ERROR_ACCESS_DENIED.</summary>
    public const int ErrorAccessDenied = 5;

    /// <summary>ERROR_IO_PENDING.</summary>
    public const int ErrorIOPending = 997;

    /// <summary>ERROR_IO_INCOMPLETE.</summary>
    public const int ErrorIOImcomplete = 996;

    private const string LibraryDll = "kernel32.dll";
}
