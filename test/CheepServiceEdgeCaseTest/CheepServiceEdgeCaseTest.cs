using Chirp.Razor;
using Xunit;

namespace CheepServiceEdgeCaseTest
{
    public class CheepServiceEdgeCaseTests
    {
        [Fact]
        public void GetCheeps_ShouldHandleNegativePageNumber()
        {
            // Arrange
            var db = new StubDBFacade();
            var service = new CheepService(db);

            // Act
            var result = service.GetCheeps(-1, 32);

            // Assert
            // The stub doesn't use the page number, so it should still return 2 cheeps.
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
    }
}