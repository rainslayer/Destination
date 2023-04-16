using System.Net;

namespace Destination.Helpers;

public static class ResponseStatusHandler
{
  private static void WriteResponseStatus(HttpStatusCode code, Stream stream)
  {
    using StreamWriter writer = new(stream, leaveOpen: true);
    writer.WriteLine($"HTTP/1.0 {(int)code} {code}");
    writer.WriteLine();
  }

  public static void WriteBadRequestResponse(Stream stream)
  {
    WriteResponseStatus(HttpStatusCode.BadRequest, stream);
  }

  public static void WriteNotFoundResponse(Stream stream)
  {
    WriteResponseStatus(HttpStatusCode.NotFound, stream);
  }

  public static void WriteOKResponse(Stream stream)
  {
    WriteResponseStatus(HttpStatusCode.OK, stream);
  }
}