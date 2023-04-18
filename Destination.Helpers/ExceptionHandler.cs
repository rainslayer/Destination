namespace Destination.Helpers;

public static class ExceptionHandler
{
  private static string _previousException = String.Empty;
  private static readonly object _mutex = new();

  public static void HandleGlobalException(Exception ex)
  {
    lock (_mutex)
    {
      if (_previousException != ex.Message)
      {
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine(ex);
      }

      _previousException = ex.Message;
    }
  }
}