using Sport_Retake_scheuer.Repository;
using Sport_Retake_scheuer.Service;

namespace Sport_Retake_scheuer;
using Sport_Retake_scheuer.Controller;


public class Router
{
    public static HttpResponse Route(HttpRequest request)
    {
        var userRepo       = new UserRepository();
        var historyRepo    = new HistoryRepository();
        var tournamentRepo = new TournamentRepository();
        
        var tournamentService = new TournamentService(
            tournamentRepo,
            userRepo,
            historyRepo
        );
        
        var _usersController   = new UsersController(userRepo);
        var _historyController = new HistoryController(
            historyRepo,
            userRepo,
            tournamentService
        );

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