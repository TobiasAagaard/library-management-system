using System.Collections.Generic;
using CommunityToolkit.Mvvm.ComponentModel;
using Lms.Domain.Models;

namespace Lms.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Library _library;

    public MainViewModel()
    {
        _library = new Library
        {
            Id = 1,
            Name = "Tech Colleage Library",
            Books = new List<Book>
            {
                new Book { Id = 1, Title = "Book 1", Author = "Author 1" },
                new Book { Id = 2, Title = "Book 2", Author = "Author 2" },
                new Book { Id = 3, Title = "Book 3", Author = "Author 3" }
            }
        };
    }

    public string LibraryName => _library.Name;

    public int BookCount => _library.Books.Count;
}
