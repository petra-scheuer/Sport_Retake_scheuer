using Newtonsoft.Json;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Interfaces;
using Sport_Retake_scheuer.Service;

namespace Sport_Retake_scheuer.Controller;

public class HistoryController
{
    private readonly IHistoryInterface _historyRepository;
    private readonly IUserInterface _userRepository;
    public HistoryController(IHistoryInterface historyRepository, IUserInterface userRepository)
    {
        _historyRepository = historyRepository;
        _userRepository = userRepository;
    }

    public HttpResponse Handle(HttpRequest request)
    {
        if (request.Method == "GET" && request.Path == "/history")
        {
            return GetUserHistory(request);
        }
        else if (request.Method == "POST" && request.Path == "/history")
        {
            return AddHistoryEntry(request);
        }
        else if (request.Method == "PUT" && request.Path == "/history")
        {
            Console.WriteLine("Muss implementiert werden");
        }
        
        else if (request.Method == "DELETE" && request.Path == "/history")
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

    private HttpResponse GetUserHistory(HttpRequest request)
    {
        try
        {
            string jsonBody = request.Body;
            var getHistoryDto = JsonConvert.DeserializeObject<GetHistoryDto>(jsonBody);

            if (getHistoryDto == null)
            {
                throw new Exception("Deserialisierung fehlgeschlagen");
            }

            var username = getHistoryDto.Username;
            var token = getHistoryDto.Token;

            bool authcheck = _userRepository.AuthByUsernameAndToken(username, token);
            if (authcheck == false)
            {
                return new HttpResponse
                {
                    StatusCode = 400,
                    ContentType = "text/plain",
                    Body = "Fehler beim Authentifizieren"
                };
            }

            var UserHistory = _historyRepository.GetUserHistory(username);
            if (UserHistory == null)
            {
                return new HttpResponse
                {
                    StatusCode = 500,
                    ContentType = "text/plain",
                    Body = "Fehler beim Aufrufen der Historie"
                };
            }

            Console.WriteLine($"User: {username}, UserHistory: {JsonConvert.SerializeObject(UserHistory)}");

            return new HttpResponse
            {
                StatusCode = 200,
                ContentType = "text/plain",
                Body = JsonConvert.SerializeObject(UserHistory)
            };
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler im GetUserHistory: " + ex);
            return new HttpResponse
            {
                StatusCode = 500,
                ContentType = "text/plain",
                Body = "Interner Serverfehler"
            };
        }
    }

    private HttpResponse AddHistoryEntry(HttpRequest request)
    {
        string jsonBody = request.Body;
        var HistoryEntryDto = JsonConvert.DeserializeObject<HistoryEntryDto>(jsonBody);
        if (HistoryEntryDto == null)
        {
            throw new Exception("Deserialisierung fehlgeschlagen");
        }

        var username = HistoryEntryDto.Username;
        var pushupcount = HistoryEntryDto.PushupCount;
        var duration = HistoryEntryDto.Duration;
        var token = HistoryEntryDto.Token;

        bool authcheck = _userRepository.AuthByUsernameAndToken(username, token);
        if (authcheck == false)
        {
            return new HttpResponse
            {
                StatusCode = 400,
                ContentType = "text/plain",
                Body = "Fehler beim Authentifizieren"
            };
        }
        bool added = TournamentService.AddPushupRecord(HistoryEntryDto);
        if (added == false || added == null)
        {
            return new HttpResponse
            {
                StatusCode = 500,
                ContentType = "text/plain",
                Body = "Fehler beim Aufrufen der Historie"
            };
        }
        return new HttpResponse
        {
            StatusCode = 200,
            ContentType = "text/plain",
            Body = "Training erfolgreich hinzugefügt"
        };
        
    }
}