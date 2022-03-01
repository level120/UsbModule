# USB Communication for C#

# Requirement

* .Net >= 6.0
* Windows >= 7.0
* [Not supported] ~~Linux~~
* [Not supported] ~~MacOS~~

# Usage

You can use on the Console, WinForm and WPF application!

```cs
// Enumerates usb devices under the device set up class with flag
var devices = UsbCommunicationManager.GetDevices(
    DeviceSetupClasses.UsbPrinter, SetupApi.Digcf.DeviceInterface | SetupApi.Digcf.AllClasses);

// Choose device
var device = devices.Values
    .FirstOrDefault(device => device.Path?.Contains(id, StringComparison.OrdinalIgnoreCase) ?? false);

// Open communication manager
using var manager = UsbCommunicationManager.Open(device);

// Fail to get handle
if (manager == null)
{
    throw new Win32Exception("Handle을 가져올 수 없습니다.");
}

// Writing to USB(byte[])
var isSuccess = manager.Write(command);
Console.WriteLine($"Write result : {isSuccess}");

// Reading from USB(byte[])
var readString = manager.Read();
Console.WriteLine(Encoding.Default.GetString(readString));
```