namespace Lms.Application.Models;

public class User
{
    public const int BorrowLimit = 4;
    public Guid Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName => $"{FirstName} {LastName}".Trim();
    public List<Book> BorrowedBooks { get; set; } = new List<Book>();
    public virtual bool BorrowBook(Book book) => TryBorrow(book, BorrowLimit);
    protected bool TryBorrow(Book book, int limit)
    {
        if (!book.IsAvailable || BorrowedBooks.Count >= limit)
        {
            return false;
        }

        book.MarkAsBorrowed();
        BorrowedBooks.Add(book);
        return true;
    }
    public bool ReturnBook(Book book)
    {
        if (!BorrowedBooks.Remove(book))
        {
            return false;
        }

        book.MarkAsReturned();
        return true;
    }

    public void DisplayBorrowedBooks(OutputWriter? write = null)
    {
        write ??= Console.WriteLine;

        if (BorrowedBooks.Count == 0)
        {
            write($"{DisplayName} has no borrowed books.");
            return;
        }

        write($"{DisplayName} has borrowed:");
        foreach (var book in BorrowedBooks)
        {
            book.DisplayInfo(write);
        }
    }
}
