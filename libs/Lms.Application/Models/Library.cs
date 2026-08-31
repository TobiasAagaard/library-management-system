namespace Lms.Application.Models;

public class Library
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new List<Book>();
    public List<User> Users { get; set; } = new List<User>();

    public void AddBook(Book book) => Books.Add(book);

    public bool RemoveBook(Book book) => Books.Remove(book);

    public void RegisterUser(User user) => Users.Add(user);

    /// <summary>
    /// Generic search: the caller passes the match condition as a function,
    /// so the library does not need one method per search criteria.
    /// </summary>
    public Book? FindBook(Func<Book, bool> predicate) => Books.FirstOrDefault(predicate);

    public Book? FindBookByISBN(string isbn) => FindBook(b => b.ISBN == isbn);

    public void DisplayAllBooks(OutputWriter? write = null)
    {
        write ??= Console.WriteLine;
        foreach (var book in Books)
        {
            book.DisplayInfo(write);
        }
    }

    public void DisplayAvailableBooks(OutputWriter? write = null)
    {
        write ??= Console.WriteLine;
        foreach (var book in Books.Where(b => b.IsAvailable))
        {
            book.DisplayInfo(write);
        }
    }
}
