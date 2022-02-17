using System.Runtime.Versioning;

namespace UsbModule.Win32;

/// <summary>
/// 열거형 정의.
/// </summary>
[SupportedOSPlatform("windows")]
public partial class SetupApi
{
    /// <summary>
    /// DIGCF.
    /// </summary>
    [Flags]
    public enum Digcf : uint
    {
        /// <summary>
        /// Error.
        /// </summary>
        None = 0,

        /// <summary>
        /// 시스템의 기본 디바이스 클래스만 반환합니다.
        /// </summary>
        Default = 1,

        /// <summary>
        /// 현재 시스템에 장착된 장치만 반환합니다.
        /// </summary>
        Present = 1 << 1,

        /// <summary>
        /// 시스템에 설치된 모든 디바이스 인터페이스 클래스를 반환합니다.<br/>
        /// 이 설정에는 현재 존재하지 않는 장치를 포함합니다.
        /// </summary>
        AllClasses = 1 << 2,

        /// <summary>
        /// 현재 하드웨어 프로파일만 반환합니다.
        /// </summary>
        Profile = 1 << 3,

        /// <summary>
        /// 지정된 장치 인터페이스 클래스에 대한 장치를 반환합니다.<br/>
        /// 열거자 매개 변수가 장치 인스턴스 ID를 지정하는 경우, 플래그 매개 변수에 이 플래그를 설정해야 합니다.<br/>
        /// Note: 장치 인스턴스 ID는 <![CDATA[<device-ID>\<instance-specific-ID>]]> 형태입니다.
        /// ex:. <![CDATA[PCI\VEN_1000&DEV_0001&SUBSYS_00000000&REV_02\1&08]]>
        /// </summary>
        DeviceInterface = 1 << 4,
    }
}
