using Destination.Handlers;
using Destination.Server;

const UInt16 port = 80;
IRequestHandler handler = new ControllerHandler(typeof(Program).Assembly);

Server server = new(port, handler);

await server.RunAsync().ConfigureAwait(false);