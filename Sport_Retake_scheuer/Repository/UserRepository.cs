using Sport_Retake_scheuer.Config;
using System.Data;
using Sport_Retake_scheuer.Interfaces;

namespace Sport_Retake_scheuer.Repository;

public class UserRepository : IUserInterface
{
    
    public bool CreateUser(string username, string password)
    {
        if (password == "" || password == null)
        {
            throw new ArgumentException("Password darf nicht leer oder null sein.");
        }
        string hashedPassword = HashPassword(password);

        if (username == "" || username == null)
        {
            throw new ArgumentException("Username darf nicht leer oder null sein.");
        }
        var createdAt = DateTime.Now;
        const string sql = @"INSERT INTO users (created_at, username, password, token,  elo)
                                 VALUES (@c, @u, @p, '', 100)";
        
        DatabaseConnection.ExecuteNonQueryWithParameters(sql,("c", createdAt), ("u", username), ("p", hashedPassword));
        return true;
    }

    public bool AuthUser(string username, string password)
    {
        if (password == "" || password == null)
        {
            throw new ArgumentException("Password darf nicht leer oder null sein.");
        }

        if (username == "" || username == null)
        {
            throw new ArgumentException("Username darf nicht leer oder null sein.");
        }
        
        const string sql = @"Select * from users where username = @u";

        DataTable dataTable = DatabaseConnection.ExecuteQueryWithParameters(sql, ("u", username));
        
        if (dataTable.Rows.Count == 0)
        {
            return false;
        }
        string? storedPasswordHash = dataTable.Rows[0]["password"].ToString(); // anm. string? -> das ? bedeutet, dass es auch ein Nullable wert sein kann. Hilft um Compiler Warnings zu vermeiden

        if (storedPasswordHash == "" || storedPasswordHash == null)
        {
            return false;
        }
        
        bool isValidPassword = VerifyPassword(password, storedPasswordHash);
        return isValidPassword;
    }

    private bool VerifyPassword(string password, string? storedPasswordHash)
    {
        return BCrypt.Net.BCrypt.Verify(password, storedPasswordHash);
    }

    private string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    public bool UpdateToken(string username, string token)
    {
        try
        {
            const string sql = @"UPDATE users SET token = @token WHERE username = @u";
            DatabaseConnection.ExecuteNonQueryWithParameters(sql, ("token", token), ("u", username));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e); //
            throw;
        }
    }
    

}

