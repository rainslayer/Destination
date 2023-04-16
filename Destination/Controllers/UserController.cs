using Destination.Handlers;

namespace Destination.Controllers;

public record User(int Id, string Name);

public class UserController : IController
{
   public User[] Index()
   {
     return new[]
     {
       new User(1, "Don Rumata"),
       new User(2, "Bruce Willis"),
     };
   }
}