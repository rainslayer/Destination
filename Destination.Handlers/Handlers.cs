using System.Net.Sockets;
using Destination.Helpers;

namespace Destination.Handlers;

public interface IRequestHandler
{
  Task HandleAsync(NetworkStream stream, Request request);
}