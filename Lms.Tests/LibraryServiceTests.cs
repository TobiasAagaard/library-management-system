using Lms.Application.Models;
using Lms.Application.Repositories;
using Lms.Application.Services;

namespace Lms.Tests;

public class LibraryServiceTests
{
    private readonly ILibraryService _service;

    public LibraryServiceTests()
    {
        _service = new LibraryService(new LibraryRepository(), new BookRepository());
    }

    private async Task<(Library library, User user, Book book)> SetUpLibraryAsync()
    {
        var library = await _service.GetOrCreateLibraryAsync("Test Library");
        var user = await _service.RegisterUserAsync(library.Id, new User { FirstName = "Test" });
        var book = await _service.AddBookAsync(library.Id, new Book { Title = "Clean Code", ISBN = "111" });
        return (library, user, book);
    }

    [Fact]
    public async Task GetOrCreateLibraryAsync_SameName_ReturnsSameLibrary()
    {
        var first = await _service.GetOrCreateLibraryAsync("Test Library");
        var second = await _service.GetOrCreateLibraryAsync("Test Library");

        Assert.Equal(first.Id, second.Id);
    }

    [Fact]
    public async Task AddBookAsync_BookShowsUpInLibrary()
    {
        var (library, _, book) = await SetUpLibraryAsync();

        var books = await _service.GetAllBooksAsync(library.Id);

        Assert.Contains(books, b => b.Id == book.Id);
    }

    [Fact]
    public async Task BorrowBookAsync_AvailableBook_SucceedsAndBookBecomesUnavailable()
    {
        var (library, user, book) = await SetUpLibraryAsync();

        var result = await _service.BorrowBookAsync(library.Id, user.Id, book.Id);

        Assert.True(result);
        Assert.False(book.IsAvailable);

        var borrowed = await _service.GetBorrowedBooksAsync(library.Id, user.Id);
        Assert.Contains(borrowed, b => b.Id == book.Id);
    }

    [Fact]
    public async Task BorrowBookAsync_AlreadyBorrowed_Fails()
    {
        var (library, user, book) = await SetUpLibraryAsync();
        var otherUser = await _service.RegisterUserAsync(library.Id, new User { FirstName = "Other" });
        await _service.BorrowBookAsync(library.Id, otherUser.Id, book.Id);

        var result = await _service.BorrowBookAsync(library.Id, user.Id, book.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task ReturnBookAsync_BorrowedBook_SucceedsAndBookBecomesAvailable()
    {
        var (library, user, book) = await SetUpLibraryAsync();
        await _service.BorrowBookAsync(library.Id, user.Id, book.Id);

        var result = await _service.ReturnBookAsync(library.Id, user.Id, book.Id);

        Assert.True(result);
        Assert.True(book.IsAvailable);
        Assert.Empty(await _service.GetBorrowedBooksAsync(library.Id, user.Id));
    }

    [Fact]
    public async Task ReturnBookAsync_BookNotBorrowedByUser_Fails()
    {
        var (library, user, book) = await SetUpLibraryAsync();

        var result = await _service.ReturnBookAsync(library.Id, user.Id, book.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task GetAvailableBooksAsync_ExcludesBorrowedBooks()
    {
        var (library, user, book) = await SetUpLibraryAsync();
        await _service.AddBookAsync(library.Id, new Book { Title = "Other Book", ISBN = "222" });
        await _service.BorrowBookAsync(library.Id, user.Id, book.Id);

        var available = await _service.GetAvailableBooksAsync(library.Id);

        var remaining = Assert.Single(available);
        Assert.Equal("Other Book", remaining.Title);
    }

    [Fact]
    public async Task FindBookByIsbnAsync_ReturnsMatchingBook()
    {
        var (library, _, book) = await SetUpLibraryAsync();

        var found = await _service.FindBookByIsbnAsync(library.Id, "111");

        Assert.NotNull(found);
        Assert.Equal(book.Id, found.Id);
    }

    [Fact]
    public async Task FindBookAsync_WithPredicate_ReturnsMatchingBook()
    {
        var (library, _, book) = await SetUpLibraryAsync();

        var found = await _service.FindBookAsync(library.Id, b => b.Title == "Clean Code");

        Assert.NotNull(found);
        Assert.Equal(book.Id, found.Id);
    }

    [Fact]
    public async Task RemoveBookAsync_BorrowedBook_Fails()
    {
        var (library, user, book) = await SetUpLibraryAsync();
        await _service.BorrowBookAsync(library.Id, user.Id, book.Id);

        var result = await _service.RemoveBookAsync(library.Id, book.Id);

        Assert.False(result);
    }

    [Fact]
    public async Task RemoveBookAsync_AvailableBook_Succeeds()
    {
        var (library, _, book) = await SetUpLibraryAsync();

        var result = await _service.RemoveBookAsync(library.Id, book.Id);

        Assert.True(result);
        Assert.Empty(await _service.GetAllBooksAsync(library.Id));
    }
}
