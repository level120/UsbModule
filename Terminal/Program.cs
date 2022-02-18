using System.Text;
using UsbModule;
using UsbModule.Win32.Identifier;

var name = "MIP-001";

var id = "3A21";

var command = new byte[]
{
    0x1d, 0x49, 0x02,
};

using var manager = UsbCommunicationManager.Open(DeviceSetupClasses.UsbPrinter, name, id);

var write = manager.Write(command);

Console.Write(write);

var read = manager.Read();

Console.Write(Encoding.Default.GetString(read));
