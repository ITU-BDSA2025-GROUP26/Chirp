using Chirp.Razor;

namespace CheepServiceTest{

    public class CheepServiceTest
    {
        [Fact]
        public void GetCheeps_ReturnsData_FromDatabase()
        {
            //Arrange
            var db = new StubDBFacade();
            var service = new CheepService(db);

            //Act
            var results = service.GetCheeps(12,32);

            //Assert
            Assert.Equal(2, results.Count());
        }

        [Fact]
        public void GetCheepsFromAuthor_ReturnsCheepsByAuthor()
        {
            //Arrange
            var db = new StubDBFacade();
            var service = new CheepService(db);
            var author = "Bo";

            //Act
            var result = service.GetCheepsFromAuthor(author, 12, 32);

            //Assert
            Assert.Single(result);
            Assert.Contains(result,c => c.Author == author);

        }
    }
}