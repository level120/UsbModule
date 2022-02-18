#pragma warning disable CA1028 // Enum Storage should be Int32
#pragma warning disable CA1052 // Static holder types should be Static or NotInheritable
#pragma warning disable RCS1135 // Declare enum member with zero value (when enum has FlagsAttribute).
#pragma warning disable SA1602 // Enumeration items should be documented
#pragma warning disable CS1591 // Missing XML comment for publicly visible type or member

namespace UsbModule.Win32.Spool;

/// <summary>
/// 열거형 정의.
/// </summary>
public partial class WinSpool
{
    /// <summary>
    /// Falgs.
    /// </summary>
    [Flags]
    public enum PrinterOptions
    {
        /// <summary>
        /// Local에 설치된 프린터의 목록을 가져옵니다.
        /// </summary>
        PrinterEnumLocal = 1 << 1,

        /// <summary>
        /// 이전에 연결했던 적이 있는 프린터의 목록을 가져옵니다.
        /// </summary>
        PrinterEnumConnections = 1 << 2,

        /// <summary>
        /// Name 매개변수로 지정한 이름을 갖는 프린터 목록을 가져옵니다.
        /// </summary>
        PrinterEnumName = 1 << 3,

        /// <summary>
        /// 네트워크 위치에 있는 프린터 및 프린터 서버의 목록을 가져옵니다(Level 1).
        /// </summary>
        PrinterEnumRemote = 1 << 4,

        /// <summary>
        /// 공유된 프린터의 목록을 가져옵니다.
        /// </summary>
        PrinterEnumShared = 1 << 5,

        /// <summary>
        /// 네트워크 위치에 있는 프린터의 목록을 가져옵니다(Level 1).
        /// </summary>
        PrinterEnumNetwork = 1 << 6,
    }
}
