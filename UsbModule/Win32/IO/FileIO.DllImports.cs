using System.Runtime.InteropServices;
using System.Runtime.Versioning;

namespace UsbModule.Win32.IO;

/// <summary>
/// File I/O.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class FileIO
{
    [DllImport(LibraryDll, SetLastError = true, CharSet = CharSet.Unicode)]
    internal static extern IntPtr CreateFile(
        string lpFileName,
        FileAccess dwDesiredAccess,
        FileShareMode dwShareMode,
        IntPtr lpSecurityAttributes,
        FileCreationDisposition dwCreationDisposition,
        FileAttributes dwFlagsAndAttributes,
        IntPtr hTemplateFile);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern bool CloseHandle(IntPtr hObject);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern bool GetOverlappedResult(
        IntPtr hFile,
        /* IntPtr */ ref NativeOverlapped lpOverlapped,
        out uint nNumberOfBytesTransferred,
        bool bWait);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern bool WriteFile(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        ref NativeOverlapped lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern int WriteFileEx(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        int nNumberOfBytesToWrite,
        ref NativeOverlapped lpOverlapped,
        [MarshalAs(UnmanagedType.FunctionPtr)]
        IOCompletionCallback callback);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern bool ReadFile(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        uint nNumberOfBytesToRead,
        out uint lpNumberOfBytesRead,
        ref NativeOverlapped lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern int ReadFileEx(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        int nNumberOfBytesToRead,
        ref NativeOverlapped lpOverlapped,
        [MarshalAs(UnmanagedType.FunctionPtr)]
        IOCompletionCallback callback);

    [DllImport(LibraryDll)]
    internal static extern bool CancelIo(IntPtr hFile);
}
