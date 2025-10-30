using Chirp.Web;
using Xunit;

namespace Chirp.Web.Tests;

public class PagedResultsTests
{
    [Theory]
    [InlineData(0, 10, 0)]
    [InlineData(1, 10, 1)]
    [InlineData(10, 10, 1)]
    [InlineData(11, 10, 2)]
    [InlineData(32, 32, 1)]
    [InlineData(33, 32, 2)]
    public void TotalPages_ComputesCeiling_GivenTotalCountAndPageSize(int totalCount, int pageSize, int expectedPages)
    {
        // Arrange
        var pr = new PagedResults<int> { Page = 1, PageSize = pageSize, TotalCount = totalCount };

        // Act
        var totalPages = pr.TotalPages;

        // Assert
        Assert.Equal(expectedPages, totalPages);
    }
    
    [Theory]
    // page, totalCount, pageSize, hasPrev, hasNext
    [InlineData(1, 100, 10, false, true)]
    [InlineData(2, 100, 10, true,  true)]
    [InlineData(10, 100, 10, true,  false)]
    [InlineData(3, 25, 10,  true,  false)] // ceil(25/10)=3 → last page
    [InlineData(1, 0, 10,   false, false)]  // no items → TotalPages=0 → Page<TotalPages is false
    public void HasPrevious_HasNext_AreCorrect(int page, int totalCount, int pageSize, bool expectedPrev, bool expectedNext)
    {
        // Arrange
        var pr = new PagedResults<int> { Page = page, PageSize = pageSize, TotalCount = totalCount };

        // Act
        var hasPrev = pr.HasPrevious;
        var hasNext = pr.HasNext;

        // Assert
        Assert.Equal(expectedPrev, hasPrev);
        Assert.Equal(expectedNext, hasNext);
    }
}