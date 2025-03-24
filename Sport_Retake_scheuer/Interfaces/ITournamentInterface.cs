using Sport_Retake_scheuer.DatalayerTransferObjects;

namespace Sport_Retake_scheuer.Interfaces;

public interface ITournamentInterface
{
    TournamentDto GetActiveTournament();

    void CreateTournament(DateTime startTime);

    void UpdateTournament(TournamentDto tournament);
}