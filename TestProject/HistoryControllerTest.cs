using Moq;
using Newtonsoft.Json;
using NUnit.Framework;
using Sport_Retake_scheuer;
using Sport_Retake_scheuer.Controller;
using Sport_Retake_scheuer.DatalayerTransferObjects;
using Sport_Retake_scheuer.Interfaces;
using Sport_Retake_scheuer.Service;

namespace TestProject
{
    [TestFixture]
    public class HistoryControllerTests
    {
        private Mock<IHistoryInterface> _historyInterfaceMock;
        private Mock<IUserInterface> _userInterfaceMock;
        private Mock<ITournamentInterface> _tournamentInterfaceMock;
        private HistoryController _historyController;

        [SetUp]
        public void Setup()
        {
            _historyInterfaceMock = new Mock<IHistoryInterface>();
            _userInterfaceMock = new Mock<IUserInterface>();
            _tournamentInterfaceMock = new Mock<ITournamentInterface>();

            // Dummy-Turnier: Immer ein Turnier mit ID 1 zurückgeben
            _tournamentInterfaceMock
                .Setup(t => t.GetActiveTournament())
                .Returns(new TournamentDto 
                { 
                    TournamentId = 1, 
                    StartTime = DateTime.Now, 
                    IsFinished = false 
                });
            _tournamentInterfaceMock
                .Setup(t => t.CreateTournament(It.IsAny<DateTime>()))
                .Callback<DateTime>(dt => { /* evtl. Dummy-Logik */ });

            // Durch den Konstruktoraufruf werden die statischen Felder gesetzt:
            new TournamentService(_tournamentInterfaceMock.Object, _userInterfaceMock.Object, _historyInterfaceMock.Object);

            // Jetzt kannst du deinen HistoryController erzeugen
            _historyController = new HistoryController(_historyInterfaceMock.Object, _userInterfaceMock.Object);
        }

        [Test]
        public void GetHistory_ValidRequest_ReturnsHistoryData()
        {
            // Arrange
            string username = "testuser";
            string token = "dummyToken";
            // Dummy-History-Daten als JSON-String:
            string dummyHistoryData = "[{\"pushup_count\":10,\"duration\":120}, {\"pushup_count\":15,\"duration\":120}]";

            _historyInterfaceMock
                .Setup(h => h.GetUserHistory(username))
                .Returns(dummyHistoryData);

            _userInterfaceMock
                .Setup(u => u.AuthByUsernameAndToken(username, token))
                .Returns(true);

            var request = new HttpRequest
            {
                Method = "GET",
                Path = "/history",
                Body = $"{{\"Username\":\"{username}\", \"Token\":\"{token}\"}}"
            };

            // Act:
            var response = _historyController.Handle(request);

            // Assert:
            // Da der Controller den Rückgabewert nochmals serialisiert,
            // erwarten wir JsonConvert.SerializeObject(dummyHistoryData)
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            Assert.AreEqual(JsonConvert.SerializeObject(dummyHistoryData), response.Body);
        }

        [Test]
        public void GetHistory_MissingUsername_ReturnsBadRequest()
        {
            // Arrange: Request ohne den erforderlichen Username
            var request = new HttpRequest
            {
                Method = "GET",
                Path = "/history",
                Body = "{}"
            };

            // Act:
            var response = _historyController.Handle(request);

            // Assert:
            Assert.AreEqual(400, response.StatusCode);
            Assert.AreEqual("Fehler beim Authentifizieren", response.Body);
        }

        [Test]
        public void AddTrainingItemToHistory_ValidRequest_ReturnsSuccessMessage()
        {
            // Arrange
            string username = "testuser";
            string token = "dummyToken";
            int pushupCount = 10;
            int duration = 120;

            _userInterfaceMock
                .Setup(u => u.AuthByUsernameAndToken(username, token))
                .Returns(true);

            _historyInterfaceMock
                .Setup(h => h.AddUserHistoryItem(username, pushupCount, duration, It.IsAny<int>()))
                .Returns(true);

            var request = new HttpRequest
            {
                Method = "POST",
                Path = "/history",
                Body = $"{{\"Username\":\"{username}\", \"Token\":\"{token}\", \"pushupcount\":{pushupCount}, \"duration\":{duration}}}"
            };

            // Act:
            var response = _historyController.Handle(request);

            // Assert:
            Assert.AreEqual(200, response.StatusCode);
            Assert.AreEqual("text/plain", response.ContentType);
            Assert.AreEqual("Training erfolgreich hinzugefügt", response.Body);
        }
    }
}