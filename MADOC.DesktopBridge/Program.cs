using System.Text;
using MADOC.DesktopBridge.App;
using MADOC.DesktopBridge.Protocol;

Console.InputEncoding = Encoding.UTF8;
Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);

var services = DesktopBridgeServiceFactory.CreateDefault();
var dispatcher = DesktopBridgeServiceFactory.CreateDispatcher(services);
var server = new JsonLineBridgeServer(Console.In, Console.Out, Console.Error, dispatcher);

await server.RunAsync();
