using Chirp.Razor;
using Xunit;

namespace CheepServiceEdgeCaseTest
{
    public class CheepServiceEdgeCaseTests
    {
        [Fact]
        public void GetCheeps_ShouldHandleNegativePageNumber()
        {
            // The stub doesn't use the page number, so it should still return 2 cheeps
            var db = new StubDBFacade();
            var service = new CheepService(db);

            var result = service.GetCheeps(-1, 32);

            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
        }
        
        [Fact]
        public void GetCheeps_ShouldHandleZeroPageSize()
        {
            // Even though page size = 0, stub ignores it and still returns all cheeps

            var db = new StubDBFacade();
            var service = new CheepService(db);

            var result = service.GetCheeps(1, 0);

            Assert.Equal(2, result.Count);
        }
    }
}