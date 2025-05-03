using Sport_Retake_scheuer.Config;
using System.Data;
using Sport_Retake_scheuer.DatalayerTransferObjects;
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
        
        /*const string sqlHistory = @"INSERT INTO history (username, pushup_count, duration, tournament_id)
VALUES (@username, 0, 0, 0);";
        DatabaseConnection.ExecuteNonQueryWithParameters(sqlHistory, ("u", username)); */ //anm.: ich habe es rausgenommen weil es probleme gemacht ha mit der fk relationship. ich hoff es funktioneirt trotzdem.
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
        string? storedPasswordHash = dataTable.Rows[0]["password"].ToString(); // anm. string? → bedeutet, dass es auch ein Nullable wert sein kann. Hilft, um Compiler Warnings zu vermeiden

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

    public bool ChangeUsername(string oldUsername, string newUsername)
    {
        try
        {
            const string sql = @"UPDATE users SET username=@u WHERE username = @o";
            DatabaseConnection.ExecuteNonQueryWithParameters(sql, ("u", newUsername), ("o", oldUsername));
            return true;
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public bool AuthByUsernameAndToken(string username, string token)
    {
        try
        {
            const string sql = @"SELECT token FROM users WHERE username = @u";
            DataTable dt = DatabaseConnection.ExecuteQueryWithParameters(sql, ("u", username));

            if (dt == null || dt.Rows.Count == 0)
            {
                // Kein Benutzer gefunden
                return false;
            }

            string tokenFromDb = dt.Rows[0]["token"].ToString();
            if (tokenFromDb != token)
            {
                Console.WriteLine($"Token-Mismatch: Erwartet '{token}', aber erhalten '{tokenFromDb}'.");
                return false;
            }

            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler in AuthByUsernameAndToken: " + ex.ToString());
            return false;
        }
    }

    public void UpdateElo(string username, int value)
    {
        throw new NotImplementedException();
    }
    
    public List<UserDtos> GetAllUsers()
    {
        const string sql = @"SELECT username, elo FROM users";
        var dt = DatabaseConnection.ExecuteQueryWithParameters(sql);
        var list = new List<UserDtos>();
        foreach (DataRow row in dt.Rows)
        {
            list.Add(new UserDtos
            {
                Username = row["username"].ToString(),
                Elo = Convert.ToInt32(row["elo"])
            });
        }
        return list;
    }
    

}

