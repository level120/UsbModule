# USB Printer Communication for C#

* usbprint.sys

# Requirement

* .Net >= 6.0
* Windows >= 7.0
* [Not supported] ~~Linux~~
* [Not supported] ~~MacOS~~

# Usage

You can use on the Console, WinForm and WPF application!

```cs
// My device id.
var id = "3A21";

// My device command.
var command = new byte[]
{
    0x1d, 0x49, 0x02,
};

// Enumerates usb devices under the device set up class with flag
var devices = UsbCommunicationManager.GetDevices(
    DeviceSetupClasses.UsbPrinter, SetupApi.GetClassDevsFlags.DIGCF_DEVICEINTERFACE | SetupApi.GetClassDevsFlags.DIGCF_ALLCLASSES);

// Choose device
var device = devices.FirstOrDefault(
    device => device.DeviceInstanceId?.Contains(id, StringComparison.OrdinalIgnoreCase) ?? false);

// Open communication manager
using var manager = UsbCommunicationManager.Open(device);

// Fail to get handle
if (manager.IsInvalid)
{
    throw new InvalidDataException("Invalid handle.");
}

// Writing to USB(byte[])
var isSuccess = manager.Write(command);

Console.WriteLine($"Write result : {isSuccess}");

// Reading from USB(byte[])
var readString = manager.Read();

Console.WriteLine(Encoding.Default.GetString(readString));
```
