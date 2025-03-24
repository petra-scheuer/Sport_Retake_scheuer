using System.Data;
using Newtonsoft.Json;
using Sport_Retake_scheuer.Config;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Interfaces;

namespace Sport_Retake_scheuer.Repository;

public class HistoryRepository : IHistoryInterface
{
    public string GetUserHistory(string username)
    {
        const string sql = @"SELECT * FROM history WHERE username = @u";

        DataTable dt = DatabaseConnection.ExecuteQueryWithParameters(sql, ("u", username)) as DataTable;

        if (dt == null || dt.Rows.Count == 0)
        {
            return "[]"; // Gibt ein leeres JSON-Array zurück, wenn keine Daten vorhanden sind.
        }

        // Serialisiert den DataTable-Inhalt in einen JSON-String
        string historyFromDb = JsonConvert.SerializeObject(dt);
        return historyFromDb;
    }

    public bool AddUserHistoryItem(string username, int pushupcount, int duration, int tournamentId)
    {
        try
        {
            const string sql = @"INSERT INTO history (username, pushup_count, duration, tournament_id)
                                     VALUES (@u, @c, @d, @t)";
            DatabaseConnection.ExecuteNonQueryWithParameters(sql,
                ("u", username),
                ("c", pushupcount),
                ("d", duration),
                ("t", tournamentId));
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine("Fehler beim Hinzufügen eines History-Eintrags: " + ex);
            return false;
        }
    }
    public List<HistoryEntryDto> GetRecordsByTournamentId(int tournamentId)
    {
        throw new NotImplementedException();
    }

}