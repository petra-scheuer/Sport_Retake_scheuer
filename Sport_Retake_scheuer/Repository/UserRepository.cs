using Sport_Retake_scheuer.Config;

namespace Sport_Retake_scheuer.Repository;

public class UserRepository
{
    public static bool CreateUser(string username, string password)
    {
        if (password == "" || password == null)
        {
            throw new ArgumentException("Password darf nicht leer oder null sein.");
        }
        string hashedpassword =  Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password));

        if (username == "" || username == null)
        {
            throw new ArgumentException("Username darf nicht leer oder null sein.");
        }
        var createdAt = DateTime.Now;
        const string sql = @"INSERT INTO users (created_at, username, password, token,  elo)
                                 VALUES (@c, @u, @p, '', 100)";
        
        DatabaseConnection.ExecuteNonQueryWithParameters(sql,("c", createdAt), ("u", username), ("p", hashedpassword));
        return true;
    }

    
}

