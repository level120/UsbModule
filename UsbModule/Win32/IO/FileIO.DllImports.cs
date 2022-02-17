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
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CloseHandle(IntPtr hObject);

    [DllImport(LibraryDll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool GetOverlappedResult(
        IntPtr hFile,
        /* IntPtr */ ref NativeOverlapped lpOverlapped,
        out uint nNumberOfBytesTransferred,
        bool bWait);

    [DllImport(LibraryDll, SetLastError = true, EntryPoint = "WriteFile")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WriteFile0(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)]
        byte[] lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        NullClass lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool WriteFile(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
        uint nNumberOfBytesToWrite,
        out uint lpNumberOfBytesWritten,
        [In] ref NativeOverlapped lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern int WriteFileEx(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
        int nNumberOfBytesToWrite,
        [In] ref NativeOverlapped lpOverlapped,
        [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback callback);

    [DllImport(LibraryDll, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReadFile(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)][Out] byte[] lpBuffer,
        uint nNumberOfBytesToRead,
        out uint lpNumberOfBytesRead,
        [In] ref NativeOverlapped lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true, EntryPoint = "ReadFile")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ReadFile0(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)][Out] byte[] lpBuffer,
        uint nNumberOfBytesToRead,
        out uint lpNumberOfBytesRead,
        NullClass lpOverlapped);

    [DllImport(LibraryDll, SetLastError = true)]
    internal static extern int ReadFileEx(
        IntPtr hFile,
        [MarshalAs(UnmanagedType.LPArray)] byte[] lpBuffer,
        int nNumberOfBytesToRead,
        [In] ref NativeOverlapped lpOverlapped,
        [MarshalAs(UnmanagedType.FunctionPtr)] IOCompletionCallback callback);

    [DllImport(LibraryDll)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool CancelIo(IntPtr hFile);
}
