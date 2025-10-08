using Chirp.Razor;
using Xunit;

namespace PagingTest
{
    public class CheepServicePagingTests
    {
        [Fact]
        public void ReturnCorrectNumberOfCheeps()
        {
            var db = new StubDBFacade();
            var service = new CheepService(db);
            int page = 1;
            int pageSize = 2; //To cheeps pr. side
            
            var result = service.GetCheeps(page, pageSize);
            
            Assert.NotNull(result);
            Assert.Equal(pageSize, result.Count); // stub returnerer altid 2, matcher pageSize
        }
    }
}