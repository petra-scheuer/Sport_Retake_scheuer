using System;
using System.Collections.Generic;
using Moq;
using NUnit.Framework;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Interfaces;
using Sport_Retake_scheuer.Service;

namespace TestProject
{
    [TestFixture]
    public class TournamentServiceTests
    {
        private Mock<ITournamentInterface> _tournamentRepo;
        private Mock<IUserInterface> _userRepo;
        private Mock<IHistoryInterface> _historyRepo;
        private TournamentService _service;
        private TournamentDto _tournament;

        [SetUp]
        public void Setup()
        {
            _tournamentRepo = new Mock<ITournamentInterface>();
            _userRepo       = new Mock<IUserInterface>();
            _historyRepo    = new Mock<IHistoryInterface>();

            // Hier unbedingt StartTime setzen!
            _tournament = new TournamentDto
            {
                TournamentId = 42,
                StartTime    = DateTime.Now,
                IsFinished   = false
            };

            _tournamentRepo
                .Setup(r => r.GetActiveTournament())
                .Returns(() => _tournament);

            _service = new TournamentService(
                _tournamentRepo.Object,
                _userRepo.Object,
                _historyRepo.Object);
        }

        [Test]
        public void AddPushupRecord_LessThanTwoMinutes_DoesNotEvaluateTournament()
        {
            // Arrange: Turnier vor 1 Minute gestartet
            _tournament.StartTime = DateTime.Now.AddMinutes(-1);

            var entry = new HistoryEntryDto
            {
                Username     = "alice",
                PushupCount  = 5,
                Duration     = 30,
                Token        = "dummyToken",      // Token ist required
                TournamentId = _tournament.TournamentId
            };

            // Act
            var result = _service.AddPushupRecord(entry);

            // Assert
            Assert.IsTrue(result);
            _historyRepo.Verify(h => h.AddUserHistoryItem(
                It.IsAny<string>(),
                It.IsAny<int>(),
                It.IsAny<int>(),
                It.IsAny<int>()),
                Times.Once);
            _userRepo.Verify(u => u.UpdateElo(It.IsAny<string>(), It.IsAny<int>()),
                Times.Never);
            _tournamentRepo.Verify(t => t.UpdateTournament(It.IsAny<TournamentDto>()),
                Times.Never);
        }

        [Test]
        public void AddPushupRecord_AfterTwoMinutes_EvaluatesTournament()
        {
            // Arrange: Turnier vor 3 Minuten gestartet
            _tournament.StartTime = DateTime.Now.AddMinutes(-3);

            var entryBob = new HistoryEntryDto
            {
                Username     = "bob",
                PushupCount  = 10,
                Duration     = 60,
                Token        = "dummyToken",
                TournamentId = _tournament.TournamentId
            };

            var entryCarol = new HistoryEntryDto
            {
                Username     = "carol",
                PushupCount  = 6,
                Duration     = 60,
                Token        = "dummyToken",
                TournamentId = _tournament.TournamentId
            };

            // Simuliere bereits vorhandene Einträge
            var records = new List<HistoryEntryDto> { entryBob, entryCarol };
            _historyRepo
                .Setup(h => h.GetRecordsByTournamentId(_tournament.TournamentId))
                .Returns(records);

            // Act
            var result = _service.AddPushupRecord(entryBob);

            // Assert
            Assert.IsTrue(result);
            _historyRepo.Verify(h => h.AddUserHistoryItem(
                "bob", 10, 60, _tournament.TournamentId), Times.Once);

            // bob gewinnt → +2
            _userRepo.Verify(u => u.UpdateElo("bob", 2), Times.Once);
            // carol verliert → -1
            _userRepo.Verify(u => u.UpdateElo("carol", -1), Times.Once);

            // Turnier wurde als beendet markiert
            _tournamentRepo.Verify(t => t.UpdateTournament(
                It.Is<TournamentDto>(dt => dt.IsFinished)), Times.Once);
        }
    }
}
