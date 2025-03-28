using Sport_Retake_scheuer.DatalayerTransferObjects;

namespace Sport_Retake_scheuer.Interfaces;

public interface IHistoryInterface
{
    public string GetUserHistory(string username);
    
    public bool AddUserHistoryItem(string username, int pushupcount, int duration, int tournamentId);
    
    public  List<HistoryEntryDto>  GetRecordsByTournamentId(int tournamentId);
}