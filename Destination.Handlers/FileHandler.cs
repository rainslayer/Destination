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
  
  public async Task HandleAsync(NetworkStream stream, Request request)
  {
    await using StreamWriter writer = new(stream);
    var filePath = Path.Combine(_staticFilesPath, request.Path[1..]);
      
    if (!File.Exists(filePath))
    {
      await ResponseStatusHandler.WriteNotFoundResponse(stream).ConfigureAwait(false);
      return;
    }

    await using var fileStream = File.OpenRead(filePath);
    await fileStream.CopyToAsync(stream).ConfigureAwait(false);
  }
}