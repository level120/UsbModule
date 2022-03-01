using UsbModule.Win32;

namespace Terminal;

public static class Formatter
{
    public static object[] ApplyFormat(this IEnumerable<WinSpool.PrinterInfo2> printerInfo2s)
    {
        return printerInfo2s.Select(ApplyFormat).ToArray();
    }

    public static object ApplyFormat(this WinSpool.PrinterInfo2 printerInfo2)
    {
        return new
        {
            printerInfo2.ServerName,
            printerInfo2.PrinterName,
            printerInfo2.ShareName,
            printerInfo2.DriverName,
            printerInfo2.Comment,
            printerInfo2.Status,
        };
    }
}
