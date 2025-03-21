using Moq;
using Newtonsoft.Json;
using Sport_Retake_scheuer;
using Sport_Retake_scheuer.Controller;
using Sport_Retake_scheuer.Interfaces;

namespace TestProject
{
    [TestFixture]
    public class HistoryControllerTests
    {
        private Mock<IHistoryInterface> _historyInterfaceMock;
        private HistoryController _historyController;
        private Mock<IUserInterface> _userRepositoryMock;

        [SetUp]
        public void Setup()
        {
            _historyInterfaceMock = new Mock<IHistoryInterface>();
            _userRepositoryMock = new Mock<IUserInterface>();
            _historyController = new HistoryController(_historyInterfaceMock.Object, _userRepositoryMock.Object);
        }

       
        [Test]
        public void GetHistory_ValidRequest_ReturnsHistoryData()
        {
            // Gültiger Username, Token und Dummy-History-Daten als JSON
            string username = "testuser";
            string token = "dummyToken";
            string dummyHistoryData =
                "[{\"pushup_count\":10,\"duration\":120}, {\"pushup_count\":15,\"duration\":120}]";

            _historyInterfaceMock
                .Setup(h => h.GetUserHistory(username))
                .Returns(dummyHistoryData);

            _userRepositoryMock
                .Setup(u => u.AuthByUsernameAndToken(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string u, string t) => !string.IsNullOrEmpty(u) && !string.IsNullOrEmpty(t) && u == "testuser" && t == "dummyToken");

            var request = new HttpRequest
            {
                Method = "GET",
                Path = "/history",
                Body = $"{{\"Username\":\"{username}\", \"Token\":\"{token}\"}}"
            };

            // Act:
            var response = _historyController.Handle(request);

            // Assert:
            Assert.AreEqual(200, response.StatusCode);

            Assert.AreEqual("text/plain", response.ContentType);
            Assert.AreEqual(JsonConvert.SerializeObject(dummyHistoryData), response.Body);
        }
        
        [Test]
        public void GetHistory_MissingUsername_ReturnsBadRequest()
        {
            // Request ohne den erforderlichen Username
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
        public void AddTrainingItemToHistory_ValidRequest_ReturnsHistoryData()
        {
            // Arrange
            string username = "testuser";
            string token = "dummyToken";
            int pushupCount = 10;
            int duration = 120;

            _userRepositoryMock
                .Setup(u => u.AuthByUsernameAndToken(It.IsAny<string>(), It.IsAny<string>()))
                .Returns((string u, string t) => !string.IsNullOrEmpty(u) && !string.IsNullOrEmpty(t) && u == "testuser" && t == "dummyToken");

            _historyInterfaceMock
                .Setup(h => h.AddUserHistoryItem(username, pushupCount, duration))
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