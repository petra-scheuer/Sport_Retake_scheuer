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

    public bool AddUserHistoryItem(string username, int pushupcount, int duration)
    {
        try
        {
            const string sql = @"INSERT INTO history (username, pushup_count, duration)
                                     VALUES (@u, @c, @d)";
            DatabaseConnection.ExecuteNonQueryWithParameters(sql,
                ("u", username),
                ("c", pushupcount),
                ("d", duration));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Hinzufügen eines History-Eintrags: " + ex);
            return false;
        }
    }

}