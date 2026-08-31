using Lms.Application.Models;

namespace Lms.Application.Repositories;

public class BookRepository : IBookRepository
{
    private readonly List<Book> _books = new List<Book>();

    public async Task<IReadOnlyList<Book>> GetAllBooksAsync()
    {
        return await Task.FromResult(_books);
    }

    public async Task<Book> GetBookByIdAsync(Guid bookId)
    {
        var book = _books.FirstOrDefault(b => b.Id == bookId);
        if (book == null)
        {
            throw new KeyNotFoundException($"Book with ID {bookId} not found.");
        }

        return await Task.FromResult(book);
    }

    public async Task<Book> CreateBookAsync(Book book)
    {
        book.Id = Guid.NewGuid();
        _books.Add(book);
        return await Task.FromResult(book);
    }

    public async Task<Book> UpdateBookAsync(Book book)
    {
        var existingBook = _books.FirstOrDefault(b => b.Id == book.Id);

        if (existingBook == null)
        {
            throw new KeyNotFoundException($"Book with ID {book.Id} not found.");
        }

        existingBook.Title = book.Title;
        existingBook.ISBN = book.ISBN;
        existingBook.Authors = book.Authors;

        return await Task.FromResult(existingBook);

    }
}

