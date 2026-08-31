namespace Lms.Application.Models;

public class Book
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string ISBN { get; set; } = string.Empty;
    public bool IsAvailable { get; private set; } = true;
    public List<Author> Authors { get; set; } = new List<Author>();
    internal void MarkAsBorrowed() => IsAvailable = false;
    internal void MarkAsReturned() => IsAvailable = true;

    public void DisplayInfo(OutputWriter? write = null)
    {
        write ??= Console.WriteLine;

        var authors = Authors.Count > 0
            ? string.Join(", ", Authors.Select(a => a.DisplayName))
            : "Unknown author";

        write($"{Title} by {authors} (ISBN: {ISBN}) - {(IsAvailable ? "Available" : "Borrowed")}");
    }
}
