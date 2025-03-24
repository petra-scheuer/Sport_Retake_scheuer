using Npgsql;
using Sport_Retake_scheuer.Config;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Interfaces;
using System.Data;

public class TournamentRepository : ITournamentInterface
{
    public TournamentDto GetActiveTournament()
    {
        // SQL: Wähle ein Turnier, das noch nicht abgeschlossen ist und bei dem weniger als 2 Minuten seit dem Start vergangen sind.
        string sql = "SELECT id, created_at, endet_at " +
                     "FROM tournament " +
                     "WHERE status = false AND (NOW() - created_at) < interval '2 minutes' " +
                     "LIMIT 1;";

        DataTable dt = DatabaseConnection.ExecuteQueryWithParameters(sql);

        if (dt.Rows.Count == 0)
        {
            return null; // Kein aktives Turnier gefunden, daher null zurückgeben.
        }

        DataRow row = dt.Rows[0];
        TournamentDto tournament = new TournamentDto
        {
            TournamentId = Convert.ToInt32(row["id"]),
            StartTime = Convert.ToDateTime(row["created_at"]),
            IsFinished = false
        };

        return tournament;
    }

    public void CreateTournament(DateTime startTime)
    {
        string formattedStartTime = startTime.ToString("yyyy-MM-dd HH:mm:ss");
    
        string sql = "INSERT INTO tournament (created_at, status, endet_at) " +
                     "VALUES ('" + formattedStartTime + "', false, NULL);";

        DatabaseConnection.ExecuteNonQueryWithParameters(sql);
    }

    public void UpdateTournament(TournamentDto tournament)
    {
        Console.WriteLine("Noch nicht implementiert");

    }
    
}