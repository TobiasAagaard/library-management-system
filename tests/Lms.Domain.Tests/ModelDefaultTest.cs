using Lms.Domain.Models;
namespace Lms.Domain.Tests;

public class BookTests
{
    [Fact]
    public void NewBook_IsAvailableByDefault()
    {
        var book = new Book();

        Assert.True(book.IsAvailable);
    }

}
