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
        const string sql = @"SELECT username, pushup_count, duration, tournament_id"
                           + " FROM history WHERE tournament_id = @t";
        
        // Query ausführen und Ergebnis in DataTable einlesen
        var dt = DatabaseConnection.ExecuteQueryWithParameters (sql, ("t", tournamentId)) as DataTable;
        var records = new List<HistoryEntryDto>();

        if (dt == null || dt.Rows.Count == 0)
        {
            return records; // anm.: Leere Liste zurückgeben, wenn keine Einträge vorhanden sind.
        }
        
        // Jeder DataRow direkt in DTO umwandeln
        foreach (DataRow row in dt.Rows)
        {
            records.Add(new HistoryEntryDto
            {
                Username = row["username"].ToString(),
                Token = string.Empty,
                PushupCount = row.Field<int>("pushup_count"),
                Duration = row.Field<int>("duration"),
                TournamentId = row.Field<int>("tournament_id")
            });
        }
        return records;
    }

}