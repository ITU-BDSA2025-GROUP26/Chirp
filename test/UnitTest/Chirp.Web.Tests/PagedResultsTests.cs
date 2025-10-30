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
}