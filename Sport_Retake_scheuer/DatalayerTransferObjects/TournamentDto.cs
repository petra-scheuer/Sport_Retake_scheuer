namespace Sport_Retake_scheuer.DatalayerTransferObjects;

public class TournamentDto
{
    public int TournamentId { get; set; }
    public required DateTime StartTime { get; set; }
    public bool IsFinished { get; set; }
}