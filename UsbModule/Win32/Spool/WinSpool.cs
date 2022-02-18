using System.ComponentModel;
using System.Runtime.InteropServices;

namespace UsbModule.Win32.Spool;

/// <summary>
/// WinSpool.
/// </summary>
public partial class WinSpool
{
    private const int ErrorInsufficientBuffer = 122;

    /// <summary>
    /// Printer 목록을 반환합니다.
    /// </summary>
    /// <param name="flags">Flags.</param>
    /// <param name="serverName">Optional, 장치를 검색할 서버이름.</param>
    /// <returns>Printer 목록.</returns>
    /// <exception cref="Win32Exception">프린터 목록 조회 중 오류가 생긴 경우.</exception>
    public static PrinterInfo2[] GetPrinters(PrinterOptions flags, string? serverName = null)
    {
        uint cbNeeded = 0;
        uint cReturned = 0;

        if (EnumPrinters(flags, serverName, 2, IntPtr.Zero, 0, ref cbNeeded, ref cReturned))
        {
            return Array.Empty<PrinterInfo2>();
        }

        var lastWin32Error = Marshal.GetLastWin32Error();

        if (lastWin32Error != ErrorInsufficientBuffer)
        {
            throw new Win32Exception(lastWin32Error);
        }

        var pAddr = Marshal.AllocHGlobal((int)cbNeeded);

        if (!EnumPrinters(flags, serverName, 2, pAddr, cbNeeded, ref cbNeeded, ref cReturned))
        {
            return Array.Empty<PrinterInfo2>();
        }

        var printerInfo2 = new PrinterInfo2[cReturned];
        var offset = Environment.Is64BitOperatingSystem
            ? pAddr.ToInt64()
            : pAddr.ToInt32();

        var increment = Marshal.SizeOf(typeof(PrinterInfo2));

        for (var i = 0; i < cReturned; i++)
        {
            printerInfo2[i] = Marshal.PtrToStructure<PrinterInfo2>(new IntPtr(offset));
            offset += increment;
        }

        Marshal.FreeHGlobal(pAddr);

        return printerInfo2;
    }
}
