using System.Net.Sockets;
using System.Text;
using Destination.Helpers;

namespace Destination.Handlers;

public class DebugHandler : IRequestHandler
{
  public async Task HandleAsync(NetworkStream stream, Request request)
  {
    using StreamReader reader = new(stream);
    await using StreamWriter writer = new(stream);
    StringBuilder builder = new();

    while (reader.Peek() != -1)
    {
      var line = await reader.ReadLineAsync().ConfigureAwait(false);
      builder.AppendLine(line);
    }

    await writer.WriteLineAsync(builder.ToString()).ConfigureAwait(false);
  }
}