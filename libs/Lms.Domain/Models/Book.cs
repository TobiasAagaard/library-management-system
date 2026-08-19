namespace Lms.Domain.Models;

public class Book
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public ICollection<Author> Authors { get; set; } = new List<Author>();
}