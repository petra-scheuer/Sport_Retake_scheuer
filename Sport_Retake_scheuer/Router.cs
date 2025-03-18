namespace Sport_Retake_scheuer;
using Sport_Retake_scheuer.Controller;


public class Router
{
    public static HttpResponse Route(HttpRequest request)
    {
        if (request.Path.StartsWith("/users"))
        {
            return UsersController.Handle(request);
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