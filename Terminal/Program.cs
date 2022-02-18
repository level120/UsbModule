using System.ComponentModel;
using System.Text;
using UsbModule;
using UsbModule.Win32;
using UsbModule.Win32.Identifier;

#pragma warning disable CA1812

var id = "3A21";

var command = new byte[]
{
    0x1d, 0x49, 0x02,
};

// 장치 가져오기(Flag 조합 이용)
var devices = UsbCommunicationManager.GetDevices(
    DeviceSetupClasses.UsbPrinter, SetupApi.Digcf.DeviceInterface | SetupApi.Digcf.AllClasses);

// 통신할 장치 선택
var device = devices.Values
    .FirstOrDefault(device => device.Path?.Contains(id, StringComparison.OrdinalIgnoreCase) ?? false);

// 통신을 위한 매니저 생성
using var manager = UsbCommunicationManager.Open(device);

// 핸들 가져오기에 실패한 경우
if (manager == null)
{
    throw new Win32Exception("Handle을 가져올 수 없습니다.");
}

// USB에 데이터 쓰기(byte[])
var isSuccess = manager.Write(command);

Console.WriteLine($"Write result : {isSuccess}");

// USB로부터 데이터 읽기(byte[])
var readString = manager.Read();

Console.WriteLine(Encoding.Default.GetString(readString));
