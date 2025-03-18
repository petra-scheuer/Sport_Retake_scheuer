using Sport_Retake_scheuer.Repository;

namespace Sport_Retake_scheuer;
using Sport_Retake_scheuer.Controller;


public class Router
{
    public static HttpResponse Route(HttpRequest request)
    {
        var _usersController = new UsersController(new UserRepository());

        if (request.Path.StartsWith("/users"))
        {
            
            return _usersController.Handle(request);
        }
        if (request.Path.StartsWith("/login"))
        {
            return _usersController.Handle(request);
        }
        else
        {
            return new HttpResponse
            {
                StatusCode = 404,
                ContentType = "text/plain",
                Body = "Nicht gefunden"
            };
        }
    }
    
}