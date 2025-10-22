using Chirp.Razor;
using Xunit;
using System.Linq;

namespace SortingTest
{
    public class CheepServiceSortingTests
    {
        [Fact]
        public void ReturnCheepsInDescendingOrder()
        {
            var db = new StubDBFacade();
            var service = new CheepService(db);
            
            var result = service.GetCheeps(1, 10).ToList();
            
            Assert.NotNull(result);
            Assert.True(result.Count > 1);
            
            Assert.Equal("Bo", result[0].Author);
            Assert.Equal("Anne", result[1].Author);
        }
    }
}