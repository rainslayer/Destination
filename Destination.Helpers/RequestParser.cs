namespace Destination.Helpers;

public record Request(HttpMethod Method, string Path);

public static class RequestParser
{
  public static Request Parse(string header)
  {
    var split = header.Split(' ');
    return new Request(GetMethod(split[0]), split[1]);
  }

  private static HttpMethod GetMethod(string method)
  {
    return method switch
    {
      "GET" => HttpMethod.Get,
      "POST" => HttpMethod.Post,
      "DELETE" => HttpMethod.Delete,
      "PUT" => HttpMethod.Put,
      "PATCH" => HttpMethod.Patch,
      _ => HttpMethod.Get,
    };
  }
}