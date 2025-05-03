using System.Runtime.InteropServices;
using Newtonsoft.Json;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Interfaces;
using Sport_Retake_scheuer.Repository;

namespace Sport_Retake_scheuer.Controller;

public class UsersController
{
    public HttpResponse Handle(HttpRequest request)
    {
        if (request.Method == "GET" && request.Path == "/scoreboard")
        {
            return GetScoreboard();
        }
        else if (request.Method == "POST" && request.Path == "/users")
        {
            return RegisterUser(request);
        }
        else if (request.Method == "PUT" && request.Path == "/users")
        {
            return ChangeUser(request);
        }
        
        else if (request.Method == "POST" && request.Path == "/login")
        {
            return LoginUser(request);
        }
        
        else if (request.Method == "GET" && request.Path == "/my_user_stats") //user soll seinen ELO value und aufsummierter push up count einsehen können
        {
            Console.WriteLine("Muss implementiert werden");
        }

        


        var response = new HttpResponse
        {
            StatusCode = 200,
            ContentType = "text/plain",
            Body = "Test damit keine Compiler errors auftreten"
        };
        return response;
    }
    
    private readonly IUserInterface _userRepository;

    public UsersController(IUserInterface userRepository)
    {
        _userRepository = userRepository;
    }

    private HttpResponse RegisterUser(HttpRequest request)
    {
        string jsonBody = request.Body;

        var userDto = JsonConvert.DeserializeObject<RegisterUser>(jsonBody);
        if(userDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        
        bool created = _userRepository.CreateUser(userDto.username, userDto.password);
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
                response.Body = "Fehler beim anlegen des Users aufgetreten";
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

    private HttpResponse ChangeUser(HttpRequest request)
    {
        string jsonBody = request.Body;
        var changeUserDto = JsonConvert.DeserializeObject<ChangeUserDto>(jsonBody);

        if (changeUserDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        bool changed = _userRepository.ChangeUsername(changeUserDto.OldUsername, changeUserDto.NewUsername);
        try
        {
            if (changed)
            {
                return new HttpResponse
                {
                    StatusCode = 200,
                    ContentType = "text/plain",
                    Body = "Username erfolgreich geändert"
                };
            }
            else
            {
                return new HttpResponse
                {
                    StatusCode = 400,
                    ContentType = "text/plain",
                    Body = "Username ändern fehlgeschlagen"
                };
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler in ChangeUser: " + ex.Message);
            return new HttpResponse
            {
                StatusCode = 500,
                ContentType = "text/plain",
                Body = "Serverfehler: " + ex.Message
            };
        }
    }
    private HttpResponse LoginUser(HttpRequest request)
    {
        string jsonBody = request.Body;

        var userDto = JsonConvert.DeserializeObject<RegisterUser>(jsonBody);
        if(userDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }
        //Console.WriteLine("Deserialiserung hat funktioniert"); //anm. Bugfixing Print statement, das war es nicht 
        
        //Creedentials prüfen
        bool authentificated = _userRepository.AuthUser(userDto.username, userDto.password);
        try
        {
            if (authentificated)
            {
                string token = Guid.NewGuid().ToString();
                
                var userRepo = new UserRepository();
                bool tokenUpdated = userRepo.UpdateToken(userDto.username, token);
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
    
    private HttpResponse GetScoreboard()
    {
        // liefert alle Nutzer sortiert nach Elo absteigend
        var users = _userRepository.GetAllUsers()
            .OrderByDescending(u => u.Elo)
            .Select(u => new UserDtos { Username = u.Username, Elo = u.Elo })
            .ToList();
        string body = JsonConvert.SerializeObject(users);
        return new HttpResponse
        {
            StatusCode = 200,
            ContentType = "application/json",
            Body = body
        };
    }
    
}

