using PInvoke;
using System.Text;
using Tababular;
using Terminal;
using UsbModule;
using UsbModule.Win32;

var formatter = new TableFormatter();

/* ========= Getting printer list sample ========= */

// 프린터 장치 목록
var printers = WinSpool.GetPrinters(WinSpool.PrinterOptions.PrinterEnumLocal);

Console.WriteLine(formatter.FormatObjects(printers.ApplyFormat()));

/* ========= USB Communication sample ========= */

var id = "3A21";

var command = new byte[]
{
    0x1d, 0x49, 0x02,
};

// 장치 가져오기(Flag 조합 이용)
var devices = UsbCommunicationManager.GetDevices(
    DeviceSetupClasses.UsbPrinter, SetupApi.GetClassDevsFlags.DIGCF_DEVICEINTERFACE | SetupApi.GetClassDevsFlags.DIGCF_PRESENT);

// 통신할 장치 선택
var device = devices.FirstOrDefault(
    device => device.DeviceInstanceId?.Contains(id, StringComparison.OrdinalIgnoreCase) ?? false);

// 통신을 위한 매니저 생성
using var manager = UsbCommunicationManager.Open(device);

// 핸들 가져오기에 실패한 경우
if (manager.IsInvalid)
{
    throw new InvalidDataException("Invalid handle.");
}

// 지연시간 반복 테스트
var loop = 1;

while (loop-- > 0)
{
    // USB에 데이터 쓰기(byte[])
    var isSuccess = manager.Write(command);

    Console.WriteLine($"Write result : {isSuccess}");

    // USB로부터 데이터 읽기(byte[])
    var readString = manager.Read();

    Console.WriteLine(Encoding.Default.GetString(readString));
}
