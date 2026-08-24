using CommunityToolkit.Mvvm.ComponentModel;
using Lms.Application.Models;

namespace Lms.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{ 
    [ObservableProperty]
    private Library _library;
    
    public MainViewModel()
    {
        _library = new Library()
        {
            Id = Guid.NewGuid(),
            Name = "My Library",
            Books = new List<Book>
            {
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 1",
                    Authors = new List<Author>
                    {
                        new Author { Id = Guid.NewGuid(), FirstName = "Author", LastName = "One" }
                    }
                },
                new Book
                {
                    Id = Guid.NewGuid(),
                    Title = "Book 2",
                    Authors = new List<Author>
                    {
                        new Author { Id = Guid.NewGuid(), FirstName = "Author", LastName = "Two" }
                    }
                }
            }
        };
    }
}
