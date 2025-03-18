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
        else if (request.Method == "POST" && request.Path == "/login")
        {
            return LoginUser(request);
        }
        


        var response = new HttpResponse();
        response.StatusCode = 200;
        response.ContentType = "text/plain";
        response.Body = "Test damit keine Compiler errors auftreten";
        return response;
    }

    private static HttpResponse RegisterUser(HttpRequest request)
    {
        string jsonBody = request.Body;

        var userDto = JsonConvert.DeserializeObject<RegisterUser>(jsonBody);
        if(userDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        
        bool created = UserRepository.CreateUser(userDto.username, userDto.password);
        try
        {


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
        catch (Exception ex)
        {
            Console.WriteLine("Fehler in RegisterUser: " + ex.Message);
            return new HttpResponse
            {
                StatusCode = 500,
                ContentType = "text/plain",
                Body = "Serverfehler: " + ex.Message
            };
        }
    }
    
    private static HttpResponse LoginUser(HttpRequest request)
    {
        string jsonBody = request.Body;

        var userDto = JsonConvert.DeserializeObject<RegisterUser>(jsonBody);
        if(userDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        
        bool authentificated = UserRepository.AuthUser(userDto.username, userDto.password);
        try
        {
            if (authentificated)
            {
                string token = Guid.NewGuid().ToString();
                
                bool tokenUpdated = UserRepository.UpdateToken(userDto.username, token);
                if (!tokenUpdated)
                {
                    return new HttpResponse
                    {
                        StatusCode = 500,
                        ContentType = "text/plain",
                        Body = "Fehler beim Generieren des Tokens"
                    };
                }
                string responseBody = JsonConvert.SerializeObject(new { Token = token });
                return new HttpResponse
                {
                    StatusCode = 200,
                    ContentType = "application/json",
                    Body = responseBody
                };
            }

            var response = new HttpResponse();
            response.StatusCode = 400;
            response.ContentType = "text/plain";
            response.Body = "Ungültiger Benutzername oder Passwort";
            return response;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler in RegisterUser: " + ex.Message);
            return new HttpResponse
            {
                StatusCode = 500,
                ContentType = "text/plain",
                Body = "Serverfehler: " + ex.Message
            };
        }
    }

}

