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
  private readonly IRequestHandler _handler;
  private readonly TcpListener _listener;
  private EServerState _state;
  
  public Server(UInt16 port, IRequestHandler handler)
  {
    _handler = handler;
    _listener = new TcpListener(IPAddress.Loopback, port);
  }

  public async Task RunAsync()
  {
    _state = EServerState.Running;
    _listener.Start();

    while (_state is EServerState.Running)
    {
      try
      {
        var client = await _listener.AcceptTcpClientAsync().ConfigureAwait(false);
        var _ = ProcessClientAsync(client);
      }
      catch (Exception ex)
      {
        ExceptionHandler.HandleGlobalException(ex);
      }
    }
  }

  private async Task ProcessClientAsync(TcpClient client)
  {
    await Task.Run(async () =>
    {
      using (client)
      await using (var stream = client.GetStream())

      using (StreamReader reader = new(stream))
      {
        var firstLine = await reader.ReadLineAsync().ConfigureAwait(false);

        if (firstLine is null)
        {
          await ResponseStatusHandler.WriteBadRequestResponse(stream);
          return;
        }

        var request = RequestParser.Parse(firstLine);

        await _handler.HandleAsync(stream, request);
      }
    }).ConfigureAwait(false);
  }

  public void Stop() {
    _state = EServerState.Stopped;
    _listener.Stop();
  }
}