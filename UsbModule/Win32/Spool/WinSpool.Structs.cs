using System.Runtime.InteropServices;

#pragma warning disable SA1600 // Elements should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace UsbModule.Win32.Spool;

/// <summary>
/// 자료구조 정의.
/// </summary>
public partial class WinSpool
{
    [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
    public struct PrinterInfo2
    {
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ServerName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string PrinterName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string ShareName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string PortName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string DriverName;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Comment;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Location;
        public IntPtr DevMode;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string SepFile;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string PrintProcessor;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Datatype;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Parameters;
        public IntPtr SecurityDescriptor;
        public uint Attributes; // See note below!
        public uint Priority;
        public uint DefaultPriority;
        public uint StartTime;
        public uint UntilTime;
        public uint Status;
        public uint CJobs;
        public uint AveragePPM;
    }
}
