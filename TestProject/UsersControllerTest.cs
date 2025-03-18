using Moq;
using Sport_Retake_scheuer;
using Sport_Retake_scheuer.Controller;
using Sport_Retake_scheuer.Interfaces;

namespace TestProject
{
    [TestFixture]
    public class UsersControllerTests
    {
        private Mock<IUserInterface> _userRepositoryMock;
        private UsersController _usersController;

        [SetUp]
        public void Setup()
        {
            // Moq-Objekt für IUserInterface erzeugen
            _userRepositoryMock = new Mock<IUserInterface>();
            // Controller mit dem gemockten Repository initialisieren
            _usersController = new UsersController(_userRepositoryMock.Object);
        }

        [Test]
        public void RegisterUser_ValidUser_ReturnsSuccessResponse()
        {
           
            _userRepositoryMock
                .Setup(repo => repo.CreateUser("testuser", "testpass"))
                .Returns(true);

           
            var request = new HttpRequest
            {
                Method = "POST",
                Path = "/users",
                Body = "{\"username\":\"testuser\", \"password\":\"testpass\"}"
            };
            
            var response = _usersController.Handle(request);


            Assert.That(response.StatusCode, Is.EqualTo(200));
            Assert.That(response.Body, Is.EqualTo("Test User angelegt"));
        }
        
    }
}