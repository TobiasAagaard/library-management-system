using Lms.Application.Models;
namespace Lms.Application.Repositories;

public interface IBookRepository
{
    Task<IReadOnlyList<Book>> GetAllBooksAsync();
    Task<Book> GetBookByIdAsync(Guid bookId);
    Task<Book> CreateBookAsync(Book book);
    Task<Book> UpdateBookAsync(Book book);
}