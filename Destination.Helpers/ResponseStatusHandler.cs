using System.Net;

namespace Destination.Helpers;

public static class ResponseStatusHandler
{
  private static async Task WriteResponseStatus(HttpStatusCode code, Stream stream)
  {
    await using StreamWriter writer = new(stream, leaveOpen: true);
    await writer.WriteLineAsync($"HTTP/1.0 {(int)code} {code}").ConfigureAwait(false);
    await writer.WriteLineAsync().ConfigureAwait(false);
  }

  public static async Task WriteBadRequestResponse(Stream stream)
  {
    await WriteResponseStatus(HttpStatusCode.BadRequest, stream).ConfigureAwait(false);
  }

  public static async Task WriteNotFoundResponse(Stream stream)
  {
    await WriteResponseStatus(HttpStatusCode.NotFound, stream).ConfigureAwait(false);
  }

  public static async Task WriteOKResponse(Stream stream)
  {
    await WriteResponseStatus(HttpStatusCode.OK, stream).ConfigureAwait(false);
  }
}