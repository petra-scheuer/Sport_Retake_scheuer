namespace Sport_Retake_scheuer;

public class UsersController
{
    public static HttpResponse Handle(HttpRequest request)
    {
        Console.WriteLine("Test damit keine Compiler errors auftreten");
        
        var response = new HttpResponse();
        response.StatusCode = 200;
        response.ContentType = "text/plain";
        response.Body = "Test damit keine Compiler errors auftreten";
        return response;
    }
}