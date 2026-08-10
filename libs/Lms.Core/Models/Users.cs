namespace Lms.Core.Models;

public class Users
{
    public int Id { get; set; }
    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;
    public string DisplayName => $"{FirstName} {LastName}";
    public List <Books> BorrowedBooks { get; set; } = new();
    
}