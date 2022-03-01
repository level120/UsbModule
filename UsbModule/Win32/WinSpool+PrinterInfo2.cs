using System.Runtime.InteropServices;

namespace UsbModule.Win32;

/// <summary>
/// WinSpool.
/// </summary>
public partial class WinSpool
{
    /// <summary>
    /// The PRINTER_INFO_2 structure specifies detailed printer information.
    /// </summary>
    /// <seealso href="https://docs.microsoft.com/ko-kr/windows/win32/printdocs/printer-info-2"/>
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PrinterInfo2
    {
        /// <summary>
        /// A pointer to a null-terminated string identifying the server that controls the printer.
        /// If this string is NULL, the printer is controlled locally.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ServerName;

        /// <summary>
        /// A pointer to a null-terminated string that specifies the name of the printer.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string PrinterName;

        /// <summary>
        /// A pointer to a null-terminated string that identifies the share point for the printer.
        /// (This string is used only if the PRINTER_ATTRIBUTE_SHARED constant was set for the Attributes member.)
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ShareName;

        /// <summary>
        /// A pointer to a null-terminated string that identifies the port(s) used to transmit data to the printer.
        /// If a printer is connected to more than one port,
        /// the names of each port must be separated by commas (for example, "LPT1:,LPT2:,LPT3:").
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string PortName;

        /// <summary>
        /// A pointer to a null-terminated string that specifies the name of the printer driver.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string DriverName;

        /// <summary>
        /// A pointer to a null-terminated string that provides a brief description of the printer.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Comment;

        /// <summary>
        /// A pointer to a null-terminated string that specifies the physical location of the printer
        /// (for example, "Bldg. 38, Room 1164").
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Location;

        /// <summary>
        /// A pointer to a DEVMODE structure that defines default printer data such as the paper orientation and the resolution.
        /// </summary>
        public IntPtr DevMode;

        /// <summary>
        /// A pointer to a null-terminated string that specifies the name of the file used to create the separator page.
        /// This page is used to separate print jobs sent to the printer.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string SepFile;

        /// <summary>
        /// A pointer to a null-terminated string that specifies the name of the print processor used by the printer.
        /// You can use the EnumPrintProcessors function to obtain a list of print processors installed on a server.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string PrintProcessor;

        /// <summary>
        /// A pointer to a null-terminated string that specifies the data type used to record the print job.
        /// You can use the EnumPrintProcessorDatatypes function to obtain a list of data types supported by a specific print processor.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string DataType;

        /// <summary>
        /// A pointer to a null-terminated string that specifies the default print-processor parameters.
        /// </summary>
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Parameters;

        /// <summary>
        /// A pointer to a SECURITY_DESCRIPTOR structure for the printer. This member may be NULL.
        /// </summary>
        public IntPtr SecurityDescriptor;

        /// <summary>
        /// The printer attributes. This member can be any reasonable combination of the following values.
        /// </summary>
        public uint Attributes;

        /// <summary>
        /// A priority value that the spooler uses to route print jobs.
        /// </summary>
        public uint Priority;

        /// <summary>
        /// The default priority value assigned to each print job.
        /// </summary>
        public uint DefaultPriority;

        /// <summary>
        /// The earliest time at which the printer will print a job.
        /// This value is expressed as minutes elapsed since 12:00 AM GMT (Greenwich Mean Time).
        /// </summary>
        public uint StartTime;

        /// <summary>
        /// The latest time at which the printer will print a job.
        /// This value is expressed as minutes elapsed since 12:00 AM GMT (Greenwich Mean Time).
        /// </summary>
        public uint UntilTime;

        /// <summary>
        /// The printer status.
        /// This member can be any reasonable combination of the following values.
        /// </summary>
        public uint Status;

        /// <summary>
        /// The number of print jobs that have been queued for the printer.
        /// </summary>
        public uint CJobs;

        /// <summary>
        /// The average number of pages per minute that have been printed on the printer.
        /// </summary>
        public uint AveragePPM;
    }
}
