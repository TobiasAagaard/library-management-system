using Lms.Application.Models;

namespace Lms.Tests;

public class LibraryTests
{
    private static Book NewBook(string title, string isbn) =>
        new() { Id = Guid.NewGuid(), Title = title, ISBN = isbn };

    [Fact]
    public void AddBook_AddsBookToLibrary()
    {
        var library = new Library();
        var book = NewBook("Clean Code", "111");

        library.AddBook(book);

        Assert.Contains(book, library.Books);
    }

    [Fact]
    public void RemoveBook_RemovesBookFromLibrary()
    {
        var library = new Library();
        var book = NewBook("Clean Code", "111");
        library.AddBook(book);

        var result = library.RemoveBook(book);

        Assert.True(result);
        Assert.Empty(library.Books);
    }

    [Fact]
    public void RegisterUser_AddsUserToLibrary()
    {
        var library = new Library();
        var user = new User { FirstName = "Test" };

        library.RegisterUser(user);

        Assert.Contains(user, library.Users);
    }

    [Fact]
    public void FindBookByISBN_ExistingBook_ReturnsBook()
    {
        var library = new Library();
        var book = NewBook("Clean Code", "111");
        library.AddBook(book);
        library.AddBook(NewBook("Other Book", "222"));

        var found = library.FindBookByISBN("111");

        Assert.Same(book, found);
    }

    [Fact]
    public void FindBookByISBN_UnknownIsbn_ReturnsNull()
    {
        var library = new Library();

        var found = library.FindBookByISBN("does-not-exist");

        Assert.Null(found);
    }

    [Fact]
    public void FindBook_WithPredicate_ReturnsMatchingBook()
    {
        var library = new Library();
        var book = NewBook("Clean Code", "111");
        library.AddBook(book);

        var found = library.FindBook(b => b.Title.StartsWith("Clean"));

        Assert.Same(book, found);
    }

    [Fact]
    public void DisplayAllBooks_WritesEveryBookThroughDelegate()
    {
        var library = new Library();
        library.AddBook(NewBook("Book A", "111"));
        library.AddBook(NewBook("Book B", "222"));

        var lines = new List<string>();
        library.DisplayAllBooks(lines.Add);

        Assert.Equal(2, lines.Count);
    }

    [Fact]
    public void DisplayAvailableBooks_OnlyWritesAvailableBooks()
    {
        var library = new Library();
        var borrowed = NewBook("Borrowed Book", "111");
        var available = NewBook("Available Book", "222");
        library.AddBook(borrowed);
        library.AddBook(available);
        new User().BorrowBook(borrowed);

        var lines = new List<string>();
        library.DisplayAvailableBooks(lines.Add);

        Assert.Single(lines);
        Assert.Contains("Available Book", lines[0]);
    }
}

public class BookTests
{
    [Fact]
    public void NewBook_IsAvailableByDefault()
    {
        Assert.True(new Book().IsAvailable);
    }

    [Fact]
    public void DisplayInfo_WritesDetailsThroughDelegate()
    {
        var book = new Book
        {
            Title = "Clean Code",
            ISBN = "111",
            Authors = { new Author { FirstName = "Robert", LastName = "Martin" } },
        };

        var lines = new List<string>();
        book.DisplayInfo(lines.Add);

        var line = Assert.Single(lines);
        Assert.Contains("Clean Code", line);
        Assert.Contains("Robert Martin", line);
        Assert.Contains("111", line);
        Assert.Contains("Available", line);
    }
}
