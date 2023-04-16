using System.Net.Sockets;
using Destination.Helpers;

namespace Destination.Handlers;

public interface IRequestHandler
{
  void Handle(NetworkStream stream, Request request);
}