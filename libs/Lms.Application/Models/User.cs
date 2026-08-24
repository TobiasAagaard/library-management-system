namespace Lms.Application.Models;

public class User
{
    public Guid Id { get; set; } 
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName => $"{FirstName} {LastName}";
    public List<Book> BorrowedBooks { get; set; } = new List<Book>();
}