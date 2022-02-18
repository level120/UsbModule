using System.Runtime.InteropServices;

namespace UsbModule.Win32.Spool;

/// <summary>
/// Dll 정의.
/// </summary>
public partial class WinSpool
{
    private const string LibraryDll = "winspool.drv";

    [DllImport(LibraryDll, CharSet = CharSet.Unicode, SetLastError = true)]
    [DefaultDllImportSearchPaths(DllImportSearchPath.System32)]
    internal static extern bool EnumPrinters(
        PrinterOptions flags,
        string? name,
        uint level,
        IntPtr pPrinterEnum,
        uint cbBuf,
        ref uint pcbNeeded,
        ref uint pcReturned);
}
