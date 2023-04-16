using System.Net;
using System.Net.Sockets;
using Destination.Handlers;
using Destination.Helpers;

namespace Destination.Server;

internal enum EServerState
{
  Stopped,
  Running,
}

public class Server
{
  private readonly UInt16 _port;
  private readonly IRequestHandler _handler;
  private readonly TcpListener _listener;
  private EServerState _state;
  
  public Server(UInt16 port, IRequestHandler handler)
  {
    _port = port;
    _handler = handler;
    _listener = new(IPAddress.Loopback, _port);
  }

  public void Run()
  {
    _state = EServerState.Running;
    _listener.Start();

    while (_state is EServerState.Running)
    {
      var client = _listener.AcceptTcpClient();
      using (var stream = client.GetStream())
      using (StreamReader reader = new(stream))
      {
        var firstLine = reader.ReadLine();

        if (firstLine is null)
        {
          ResponseStatusHandler.WriteBadRequestResponse(stream);
          return;
        }
      
        var request = RequestParser.Parse(firstLine);
        _handler.Handle(stream, request);
      }
    }
  }

  public void Stop() {
    _state = EServerState.Stopped;
    _listener.Stop();
  }
}