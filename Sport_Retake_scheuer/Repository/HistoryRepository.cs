using Sport_Retake_scheuer.Config;
using Sport_Retake_scheuer.Interfaces;

namespace Sport_Retake_scheuer.Repository;

public class HistoryRepository : IHistoryInterface
{
    public string GetUserHistory(string username)
    {
        const string sql = @"SELECT * FROM history WHERE username = @u";
    
        object result = DatabaseConnection.ExecuteQueryWithParameters(sql, ("u", username));
        
        string historyFromDb = result.ToString();

        return historyFromDb;
    }
}