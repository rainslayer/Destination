using System.Net.Sockets;
using Destination.Helpers;

namespace Destination.Handlers;

public class FileHandler : IRequestHandler
{
  private readonly string _staticFilesPath;

  public FileHandler(string path)
  {
    _staticFilesPath = path;
  }
  
  public void Handle(NetworkStream stream, Request request)
  {
    using (StreamWriter writer = new(stream))
    {
      
      var filePath = Path.Combine(_staticFilesPath, request.Path[1..]);
      
      if (!File.Exists(filePath))
      {
        ResponseStatusHandler.WriteNotFoundResponse(stream);
        return;
      }
      
      using var fileStream = File.OpenRead(filePath);
      fileStream.CopyTo(stream);
    }
  }
}