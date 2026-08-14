namespace Lms.Domain.Models;

public class Library
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Book> Books { get; set; } = new();
    public List<User> Users { get; set; } = new();
}