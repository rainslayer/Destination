using System.Net.Sockets;
using System.Text;
using Destination.Helpers;

namespace Destination.Handlers;

public class DebugHandler : IRequestHandler
{
  public void Handle(NetworkStream stream, Request request)
  {
    using (StreamReader reader = new(stream))
    using (StreamWriter writer = new(stream))
    {
      StringBuilder builder = new();

      while (reader.Peek() != -1)
      {
        var line = reader.ReadLine();
        builder.AppendLine(line);
      }

      writer.WriteLine(builder.ToString());
    }
  }
}