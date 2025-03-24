using System;
using System.Collections.Generic;
using System.Linq;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Interfaces;

namespace Sport_Retake_scheuer.Service
{
    public class TournamentService 
    {
        private static ITournamentInterface _tournamentRepo;
        private static IUserInterface _userRepo;
        private static IHistoryInterface _historyRepo;
    
        public TournamentService(ITournamentInterface tournamentRepo, IUserInterface userRepo, IHistoryInterface historyRepo)
        {
            _tournamentRepo = tournamentRepo;
            var userInterface = _userRepo;
            _userRepo = userRepo;
            _historyRepo = historyRepo;
        }
    
        public static bool AddPushupRecord(HistoryEntryDto entry)
        {
            // Prüfen, ob ein aktives Turnier existiert (Turnier, das innerhalb der letzten 2 Minuten gestartet wurde und nicht abgeschlossen ist)
            var activeTournament = _tournamentRepo.GetActiveTournament();
            if (activeTournament == null)
            {
                // Neues Turnier erstellen
                _tournamentRepo.CreateTournament(DateTime.Now);
                // Nach Erstellung erneut abrufen (hier nehmen wir an, dass GetActiveTournament nun das neue Turnier liefert)
                activeTournament = _tournamentRepo.GetActiveTournament();
            }
    
            // TournamentId dem History-Eintrag zuweisen
            entry.TournamentId = activeTournament.TournamentId;
            string username = entry.Username;
            int duration = entry.Duration;
            int pushupcount = entry.PushupCount;
            int tournamentId = entry.TournamentId;
            _historyRepo.AddUserHistoryItem(username, pushupcount, duration, tournamentId);
    
            // Turnier auswerten, falls 2 Minuten seit Start vergangen sind
            if (DateTime.Now - activeTournament.StartTime >= TimeSpan.FromMinutes(2))
            {
                EvaluateTournament(activeTournament);
            }
    
            return true;
        }
    
        private static void EvaluateTournament(TournamentDto tournament)
        {
            // Alle History-Einträge dieses Turniers abrufen
            var records = _historyRepo.GetRecordsByTournamentId(tournament.TournamentId);
    
            // Gruppieren nach Username und Summe der Push-ups berechnen
            var userTotals = records.GroupBy(r => r.Username)
                .Select(g => new 
                { 
                    Username = g.Key, 
                    Total = g.Sum(r => r.PushupCount) // statt r.Count
                })
                .ToList();
    
            if (userTotals.Count == 0)
            {
                return; // Keine Einträge -> nichts zu tun
            }
    
            int max = userTotals.Max(u => u.Total);
            var winners = userTotals.Where(u => u.Total == max).ToList();
    
            if (winners.Count == 1)
            {
                // Ein eindeutiger Gewinner: Gewinner erhält +2, alle anderen -1
                _userRepo.UpdateElo(winners[0].Username, 2);
                foreach (var user in userTotals.Where(u => u.Username != winners[0].Username))
                {
                    _userRepo.UpdateElo(user.Username, -1);
                }
            }
            else
            {
                // Gleichstand: alle Gewinner erhalten +1, alle anderen -1
                foreach (var winner in winners)
                {
                    _userRepo.UpdateElo(winner.Username, 1);
                }
                foreach (var user in userTotals.Where(u => !winners.Any(w => w.Username == u.Username)))
                {
                    _userRepo.UpdateElo(user.Username, -1);
                }
            }
    
            // Turnier als abgeschlossen markieren
            tournament.IsFinished = true;
            _tournamentRepo.UpdateTournament(tournament);
        }
    }
}