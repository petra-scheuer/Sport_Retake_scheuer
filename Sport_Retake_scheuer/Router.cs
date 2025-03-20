using Sport_Retake_scheuer.Repository;

namespace Sport_Retake_scheuer;
using Sport_Retake_scheuer.Controller;


public class Router
{
    public static HttpResponse Route(HttpRequest request)
    {
        var _usersController = new UsersController(new UserRepository());
        var _historyController = new HistoryController(new HistoryRepository(), new UserRepository());

        if (request.Path.StartsWith("/users"))
        {
            return _usersController.Handle(request);
        }
        else if (request.Path.StartsWith("/login"))
        {
            return _usersController.Handle(request);
        }
        else if (request.Path.StartsWith("/history"))
        {
            return _historyController.Handle(request);
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