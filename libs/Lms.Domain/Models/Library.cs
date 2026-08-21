namespace Lms.Domain.Models;

public class Library
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public ICollection<Book> Books { get; set; } = new List<Book>();
    public ICollection<User> Users { get; set; } = new List<User>();
}