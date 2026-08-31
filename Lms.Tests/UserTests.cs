using Lms.Application.Models;

namespace Lms.Tests;

public class UserTests
{
    private static Book NewBook(string title = "Test Book") =>
        new() { Id = Guid.NewGuid(), Title = title, ISBN = "123" };

    [Fact]
    public void BorrowBook_AvailableBook_Succeeds()
    {
        var user = new User { FirstName = "Test" };
        var book = NewBook();

        var result = user.BorrowBook(book);

        Assert.True(result);
        Assert.False(book.IsAvailable);
        Assert.Contains(book, user.BorrowedBooks);
    }

    [Fact]
    public void BorrowBook_UnavailableBook_Fails()
    {
        var otherUser = new User();
        var user = new User();
        var book = NewBook();
        otherUser.BorrowBook(book);

        var result = user.BorrowBook(book);

        Assert.False(result);
        Assert.Empty(user.BorrowedBooks);
    }

    [Fact]
    public void BorrowBook_AtLimit_Fails()
    {
        var user = new User();
        for (var i = 0; i < User.BorrowLimit; i++)
        {
            Assert.True(user.BorrowBook(NewBook($"Book {i}")));
        }

        var result = user.BorrowBook(NewBook("One too many"));

        Assert.False(result);
        Assert.Equal(User.BorrowLimit, user.BorrowedBooks.Count);
    }

    [Fact]
    public void ReturnBook_BorrowedBook_Succeeds_AndBookBecomesAvailable()
    {
        var user = new User();
        var book = NewBook();
        user.BorrowBook(book);

        var result = user.ReturnBook(book);

        Assert.True(result);
        Assert.True(book.IsAvailable);
        Assert.Empty(user.BorrowedBooks);
    }

    [Fact]
    public void ReturnBook_NotBorrowed_Fails()
    {
        var user = new User();
        var book = NewBook();

        var result = user.ReturnBook(book);

        Assert.False(result);
        Assert.True(book.IsAvailable);
    }

    [Fact]
    public void DisplayBorrowedBooks_WritesThroughDelegate()
    {
        var user = new User { FirstName = "Test", LastName = "User" };
        user.BorrowBook(NewBook("Clean Code"));

        var lines = new List<string>();
        user.DisplayBorrowedBooks(lines.Add);

        Assert.Equal(2, lines.Count);
        Assert.Contains("Test User", lines[0]);
        Assert.Contains("Clean Code", lines[1]);
    }
}

public class PremiumUserTests
{
    private static Book NewBook(string title) =>
        new() { Id = Guid.NewGuid(), Title = title, ISBN = "123" };

    [Fact]
    public void BorrowBook_PremiumUser_CanBorrowOneMoreThanNormalUser()
    {
        var premiumUser = new PremiumUser();
        for (var i = 0; i < User.BorrowLimit; i++)
        {
            Assert.True(premiumUser.BorrowBook(NewBook($"Book {i}")));
        }

        Assert.True(premiumUser.BorrowBook(NewBook("Extra premium book")));
        Assert.Equal(PremiumUser.PremiumBorrowLimit, premiumUser.BorrowedBooks.Count);
    }

    [Fact]
    public void BorrowBook_PremiumUser_AtPremiumLimit_Fails()
    {
        var premiumUser = new PremiumUser();
        for (var i = 0; i < PremiumUser.PremiumBorrowLimit; i++)
        {
            premiumUser.BorrowBook(NewBook($"Book {i}"));
        }

        var result = premiumUser.BorrowBook(NewBook("One too many"));

        Assert.False(result);
        Assert.Equal(PremiumUser.PremiumBorrowLimit, premiumUser.BorrowedBooks.Count);
    }

    [Fact]
    public void BorrowBook_ThroughBaseReference_UsesPremiumLimit()
    {
        User user = new PremiumUser();
        for (var i = 0; i < User.BorrowLimit; i++)
        {
            user.BorrowBook(NewBook($"Book {i}"));
        }

        Assert.True(user.BorrowBook(NewBook("Extra premium book")));
    }
}
