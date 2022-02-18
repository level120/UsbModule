using Microsoft.Win32.SafeHandles;
using System.Runtime.InteropServices;
using System.Runtime.Versioning;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable

namespace UsbModule.Win32.IO;

/// <summary>
/// File I/O.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class FileIO
{
    [DllImport(LibraryDll, SetLastError = true, CharSet = CharSet.Unicode)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern SafeFileHandle CreateFile(
        string lpFileName,
        FileAccess dwDesiredAccess,
        FileShareMode dwShareMode,
        IntPtr lpSecurityAttributes,
        FileCreationDisposition dwCreationDisposition,
        FileAttributes dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    [DllImport(LibraryDll, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern bool GetOverlappedResult(
        IntPtr hFile,
        /* IntPtr */ ref NativeOverlapped lpOverlapped,
        out uint nNumberOfBytesTransferred,
        bool bWait);

    [DllImport(LibraryDll, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern bool WriteFile(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        ref NativeOverlapped lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern int WriteFileEx(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        int nNumberOfBytesToWrite,
        ref NativeOverlapped lpOverlapped,
        [MarshalAs(UnmanagedType.FunctionPtr)]
        IOCompletionCallback callback);

    [DllImport(LibraryDll, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern bool ReadFile(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        uint nNumberOfBytesToRead,
        out uint lpNumberOfBytesRead,
        ref NativeOverlapped lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern int ReadFileEx(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        int nNumberOfBytesToRead,
        ref NativeOverlapped lpOverlapped,
        [MarshalAs(UnmanagedType.FunctionPtr)]
        IOCompletionCallback callback);

    [DllImport(LibraryDll)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern bool CancelIo(IntPtr hFile);
}
