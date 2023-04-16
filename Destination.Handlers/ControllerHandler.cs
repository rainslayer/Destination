using System.Net.Sockets;
using System.Reflection;
using Destination.Helpers;
using Newtonsoft.Json;

namespace Destination.Handlers;

public interface IController {}

public class ControllerHandler : IRequestHandler
{
  private readonly Dictionary<string, Func<object>> _controllers;

  public ControllerHandler(Assembly controllersAssembly)
  {
    _controllers = controllersAssembly.GetTypes()
      .Where(x => typeof(IController).IsAssignableFrom(x))
      .SelectMany(Controller => Controller.GetMethods().Select(Method => new
      {
        Controller,
        Method
      }))
      .ToDictionary(
        key => GetPath(key.Controller, key.Method),
        value => GetEndpointMethod(value.Controller, value.Method)
      );
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

  public void Handle(NetworkStream stream, Request request)
  {
    if (!_controllers.TryGetValue(request.Path, out var func))
    {
      ResponseStatusHandler.WriteNotFoundResponse(stream);
      return;
    } 
    
    ResponseStatusHandler.WriteOKResponse(stream);
    WriteControllerResponse(func(), stream);
  }

  private void WriteControllerResponse(object response, Stream stream)
  {
    if (response is string str)
    {
      using StreamWriter writer = new(stream, leaveOpen: true);
      writer.Write(str);
    } else if (response is byte[] buffer)
    {
      stream.Write(buffer, 0, buffer.Length);
    }
    else
    {
      WriteControllerResponse(JsonConvert.SerializeObject(response), stream);
    }
  }
}