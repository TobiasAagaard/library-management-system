namespace Lms.Core.Models;

public class Librarys
{
    public int Id { get; set; }
    List<Books> Books { get; set; } = new();
    List<Users> Users { get; set; } = new();
}