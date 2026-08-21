namespace Lms.Domain.Models;

public class User
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName => $"{FirstName} {LastName}";
    public ICollection<Book> BorrowedBooks { get; set; } = new List<Book>();
    
}