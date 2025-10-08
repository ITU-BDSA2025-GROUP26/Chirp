using Chirp.Razor;
using Xunit;
using System.Linq;

namespace StubDBFacadeTest
{
    public class StubDBFacadeTests
    {
        [Fact]
        public void GetCheeps_ShouldReturnTwoCheeps()
        {
            // Testing if the stub always return 2 cheeps with the correct data.
            var db = new StubDBFacade();

            
            var result = db.GetCheeps(1, 32);

            
            Assert.NotNull(result);
            Assert.Equal(2, result.Count);
            Assert.Contains(result, c => c.Author == "Bo" && c.Message == "Hej Anne");
            Assert.Contains(result, c => c.Author == "Anne" && c.Message == "Hej Bo");
        }

        [Fact]
        public void GetCheepsFromAuthor_ShouldReturnOnlyMatchingAuthor()
        {
            // Testing filtering logic / are cheeps filtered by author name.
            var db = new StubDBFacade();

            
            var result = db.GetCheepsFromAuthor("Bo", 1, 32);

            
            Assert.Single(result);
            var cheep = result.First();
            Assert.Equal("Bo", cheep.Author);
            Assert.Equal("Hej Anne", cheep.Message);
        }

        [Fact]
        public void GetCheepsFromAuthor_ShouldReturnEmptyList_WhenNoMatch()
        {
            // Testing if the stub handles "no author found" correctly. 
            var db = new StubDBFacade();

            
            var result = db.GetCheepsFromAuthor("NonexistentUser", 1, 32);

            
            Assert.Empty(result);
        }
    }
}