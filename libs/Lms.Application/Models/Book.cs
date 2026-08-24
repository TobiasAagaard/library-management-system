namespace Lms.Application.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public bool IsAvailable { get; set; } = true;
    public List<Author> Authors { get; set; } = new List<Author>();

}