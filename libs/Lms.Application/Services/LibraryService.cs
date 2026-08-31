using Lms.Application.Models;
using Lms.Application.Repositories;

namespace Lms.Application.Services;

public class LibraryService : ILibraryService
{
    private readonly ILibraryRepository _libraryRepository;
    private readonly IBookRepository _bookRepository;

    public LibraryService(ILibraryRepository libraryRepository, IBookRepository bookRepository)
    {
        _libraryRepository = libraryRepository;
        _bookRepository = bookRepository;
    }

    public async Task<Library> GetOrCreateLibraryAsync(string name)
    {
        var libraries = await _libraryRepository.GetAllLibrariesAsync();
        var library = libraries.FirstOrDefault(l => l.Name == name);

        return library ?? await _libraryRepository.CreateLibraryAsync(new Library { Name = name });
    }

    public async Task<Book> AddBookAsync(Guid libraryId, Book book)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        var created = await _bookRepository.CreateBookAsync(book);

        library.AddBook(created);
        await _libraryRepository.UpdateLibraryAsync(library);

        return created;
    }

    public async Task<bool> RemoveBookAsync(Guid libraryId, Guid bookId)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        var book = library.FindBook(b => b.Id == bookId);

        if (book is null || !book.IsAvailable)
        {
            return false;
        }

        library.RemoveBook(book);
        await _libraryRepository.UpdateLibraryAsync(library);
        return true;
    }

    public async Task<User> RegisterUserAsync(Guid libraryId, User user)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);

        user.Id = Guid.NewGuid();
        library.RegisterUser(user);
        await _libraryRepository.UpdateLibraryAsync(library);

        return user;
    }

    public async Task<IReadOnlyList<Book>> GetAllBooksAsync(Guid libraryId)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        return library.Books.AsReadOnly();
    }

    public async Task<IReadOnlyList<Book>> GetAvailableBooksAsync(Guid libraryId)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        return library.Books.Where(b => b.IsAvailable).ToList();
    }

    public async Task<Book?> FindBookAsync(Guid libraryId, Func<Book, bool> predicate)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        return library.FindBook(predicate);
    }

    public Task<Book?> FindBookByIsbnAsync(Guid libraryId, string isbn) =>
        FindBookAsync(libraryId, b => b.ISBN == isbn);

    public async Task<bool> BorrowBookAsync(Guid libraryId, Guid userId, Guid bookId)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        var user = library.Users.FirstOrDefault(u => u.Id == userId);
        var book = library.FindBook(b => b.Id == bookId);

        if (user is null || book is null || !user.BorrowBook(book))
        {
            return false;
        }

        await _libraryRepository.UpdateLibraryAsync(library);
        return true;
    }

    public async Task<bool> ReturnBookAsync(Guid libraryId, Guid userId, Guid bookId)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        var user = library.Users.FirstOrDefault(u => u.Id == userId);
        var book = user?.BorrowedBooks.FirstOrDefault(b => b.Id == bookId);

        if (user is null || book is null || !user.ReturnBook(book))
        {
            return false;
        }

        await _libraryRepository.UpdateLibraryAsync(library);
        return true;
    }

    public async Task<IReadOnlyList<Book>> GetBorrowedBooksAsync(Guid libraryId, Guid userId)
    {
        var library = await _libraryRepository.GetLibraryByIdAsync(libraryId);
        var user = library.Users.FirstOrDefault(u => u.Id == userId);

        return user?.BorrowedBooks.AsReadOnly() ?? (IReadOnlyList<Book>)Array.Empty<Book>();
    }
}
