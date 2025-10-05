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




        }
    }
}