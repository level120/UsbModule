using UsbModule;
using UsbModule.Win32.Identifier;

var name = "MIP-001";

var id = "3A21";

using var manager = UsbCommunicationManager.Open(SupportGuids.UsbPrinter, name, id);

Console.Write(manager.Handle.ToString());
