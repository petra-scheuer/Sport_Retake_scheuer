namespace Sport_Retake_scheuer.DatalayerTransferObjects;

public class HistoryEntryDto
{
    public required string Username { get; set; }
    public required string Token { get; set; }
    public required int PushupCount { get; set; }
    public required int Duration { get; set; }
    
    public  int TournamentId { get; set; }
}