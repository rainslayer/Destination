using System.Collections.Concurrent;
using System.Net.Sockets;
using System.Reflection;
using Destination.Helpers;
using Newtonsoft.Json;

namespace Destination.Handlers;

public interface IController {}

public class ControllerHandler : IRequestHandler
{
  private readonly ConcurrentDictionary<string, Func<object>> _controllers;

  public ControllerHandler(Assembly controllersAssembly)
  {
    _controllers = new ConcurrentDictionary<string, Func<object>>(controllersAssembly.GetTypes()
      .AsParallel()
      .Where(x => typeof(IController).IsAssignableFrom(x))
      .SelectMany(Controller => Controller.GetMethods().Select(Method => new
      {
        Controller,
        Method
      }))
      .ToDictionary(
        key => GetPath(key.Controller, key.Method),
        value => GetEndpointMethod(value.Controller, value.Method)
      ));
  }

  private Func<object?> GetEndpointMethod(Type controller, MethodInfo method)
  {
    return () => method.Invoke(Activator.CreateInstance(controller), Array.Empty<object>());
  }

  private string GetPath(Type controller, MethodInfo method)
  {
    string name = controller.Name;
    
    if (name.EndsWith("Controller", StringComparison.InvariantCultureIgnoreCase))
    {
      name = name.Substring(0, name.Length - "Controller".Length);
    }

    if (method.Name.Equals("Index", StringComparison.InvariantCultureIgnoreCase))
    {
      return $"/{name}";
    }

    return $"/{name}/{method.Name}";
  }

  public async Task HandleAsync(NetworkStream stream, Request request)
  {
    if (!_controllers.TryGetValue(request.Path, out var func))
    {
      await ResponseStatusHandler.WriteNotFoundResponse(stream).ConfigureAwait(false);
      return;
    } 
    
    await ResponseStatusHandler.WriteOKResponse(stream).ConfigureAwait(false);
    await WriteControllerResponseAsync(func(), stream).ConfigureAwait(false);
  }

  private async Task WriteControllerResponseAsync(object response, Stream stream)
  {
    if (response is string str)
    {
      await using StreamWriter writer = new(stream, leaveOpen: true);
      await writer.WriteAsync(str).ConfigureAwait(false);
    } else if (response is byte[] buffer)
    {
      await stream.WriteAsync(buffer).ConfigureAwait(false);
    }
    else
    {
      await WriteControllerResponseAsync(JsonConvert.SerializeObject(response), stream).ConfigureAwait(false);
    }
  }
}