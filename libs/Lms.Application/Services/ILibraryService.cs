using Lms.Application.Models;

namespace Lms.Application.Services;

public interface ILibraryService
{
    Task<Library> GetOrCreateLibraryAsync(string name);
    Task<Book> AddBookAsync(Guid libraryId, Book book);
    Task<bool> RemoveBookAsync(Guid libraryId, Guid bookId);
    Task<User> RegisterUserAsync(Guid libraryId, User user);
    Task<IReadOnlyList<Book>> GetAllBooksAsync(Guid libraryId);
    Task<IReadOnlyList<Book>> GetAvailableBooksAsync(Guid libraryId);
    Task<Book?> FindBookAsync(Guid libraryId, Func<Book, bool> predicate);
    Task<Book?> FindBookByIsbnAsync(Guid libraryId, string isbn);
    Task<bool> BorrowBookAsync(Guid libraryId, Guid userId, Guid bookId);
    Task<bool> ReturnBookAsync(Guid libraryId, Guid userId, Guid bookId);
    Task<IReadOnlyList<Book>> GetBorrowedBooksAsync(Guid libraryId, Guid userId);
}
