namespace Sport_Retake_scheuer.Interfaces;

public interface IUserInterface
{
    bool CreateUser(string username, string password);
    bool AuthUser(string username, string password);
    bool UpdateToken(string username, string token);
    
    bool ChangeUsername(string oldUsername, string newUsername);
}