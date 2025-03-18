using Newtonsoft.Json;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Repository;

namespace Sport_Retake_scheuer.Controller;

public class UsersController
{
    public static HttpResponse Handle(HttpRequest request)
    {
        if (request.Method == "GET" && request.Path == "/users")
        {
            Console.WriteLine("Muss implementiert werden");
        }
        else if (request.Method == "POST" && request.Path == "/users")
        {
            return RegisterUser(request);
            
        }
        


        var response = new HttpResponse();
        response.StatusCode = 200;
        response.ContentType = "text/plain";
        response.Body = "Test damit keine Compiler errors auftreten";
        return response;
    }

    private static HttpResponse RegisterUser(HttpRequest request)
    {
        using var reader = new StreamReader(request.Body);
        string jsonBody = reader.ReadToEnd();

        var userDto = JsonConvert.DeserializeObject<RegisterUser>(jsonBody);
        
        bool created = UserRepository.CreateUser(userDto.username, userDto.password);
        
        if (created)
        {
            var response = new HttpResponse();
            response.StatusCode = 200;
            response.ContentType = "text/plain";
            response.Body = "Test User angelegt";
            return response;
        }
        else
        {
            var response = new HttpResponse();
            response.StatusCode = 400;
            response.ContentType = "text/plain";
            response.Body = "Client Probleme";
            return response;
        }
    }
    
}

