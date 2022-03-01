using System.Runtime.InteropServices;

namespace UsbModule.Win32;

/// <summary>
/// WinSpool.
/// </summary>
public partial class WinSpool
{
    private const string LibraryDll = "winspool.drv";

    /// <summary>
    /// winspool.drv - EnumPrinters.
    /// </summary>
    /// <param name="flags">
    /// The types of print objects that the function should enumerate.<br/>
    /// This value can be one or more of the following values.
    /// </param>
    /// <param name="name">
    /// If Level is 1, Flags contains PRINTER_ENUM_NAME, and Name is non-NULL,
    /// then Name is a pointer to a null-terminated string that specifies the name of the object to enumerate.
    /// This string can be the name of a server, a domain, or a print provider.<br/><br/>
    ///
    /// If Level is 1, Flags contains PRINTER_ENUM_NAME, and Name is NULL,
    /// then the function enumerates the available print providers.<br/><br/>
    ///
    /// If Level is 1, Flags contains PRINTER_ENUM_REMOTE,
    /// and Name is NULL, then the function enumerates the printers in the user's domain.<br/><br/>
    ///
    /// If Level is 2 or 5, Name is a pointer to a null-terminated string that specifies the name of a server whose printers are to be enumerated.
    /// If this string is NULL, then the function enumerates the printers installed on the local computer.<br/><br/>
    ///
    /// If Level is 4, Name should be NULL. The function always queries on the local computer.<br/><br/>
    ///
    /// When Name is NULL, setting Flags to PRINTER_ENUM_LOCAL | PRINTER_ENUM_CONNECTIONS enumerates printers that are installed on the local machine.
    /// These printers include those that are physically attached to the local machine as well as remote printers to which it has a network connection.<br/><br/>
    ///
    /// When Name is not NULL, setting Flags to PRINTER_ENUM_LOCAL | PRINTER_ENUM_NAME enumerates the local printers that are installed on the server Name.
    /// </param>
    /// <param name="level">
    /// The type of data structures pointed to by pPrinterEnum.
    /// Valid values are 1, 2, 4, and 5, which correspond to the PRINTER_INFO_1, PRINTER_INFO_2 , PRINTER_INFO_4, and PRINTER_INFO_5 data structures.<br/><br/>
    /// This value can be 1, 2, 4, or 5.
    /// </param>
    /// <param name="pPrinterEnum">
    /// A pointer to a buffer that receives an array of PRINTER_INFO_1, PRINTER_INFO_2, PRINTER_INFO_4, or PRINTER_INFO_5 structures.
    /// Each structure contains data that describes an available print object.
    /// </param>
    /// <param name="cbBuf">The size, in bytes, of the buffer pointed to by pPrinterEnum.</param>
    /// <param name="pcbNeeded">
    /// A pointer to a value that receives the number of bytes copied if the function succeeds or the number of bytes required if cbBuf is too small.
    /// </param>
    /// <param name="pcReturned">
    /// A pointer to a value that receives the number of PRINTER_INFO_1, PRINTER_INFO_2 , PRINTER_INFO_4, or PRINTER_INFO_5
    /// structures that the function returns in the array to which pPrinterEnum points.
    /// </param>
    /// <returns>
    /// If the function succeeds, the return value is a nonzero value.<br/>
    /// If the function fails, the return value is zero.
    /// </returns>
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
