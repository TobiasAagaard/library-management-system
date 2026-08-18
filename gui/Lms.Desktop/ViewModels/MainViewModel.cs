using System.Collections.Generic;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using Lms.Domain.Models;
using Lms.Infrastructure.Database;

namespace Lms.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly Library _library;

    [ObservableProperty]
    private string _databaseStatus = "Not connected";

    public MainViewModel() : this(null)
    {
        
    }

    public MainViewModel(DatabaseConnection? database)
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

        if (database is not null)
        {
            _ = ShowDatabaseStatusAsync(database);
        }
    }

    public string LibraryName => _library.Name;

    public int BookCount => _library.Books.Count;

    private async Task ShowDatabaseStatusAsync(DatabaseConnection database)
    {
        DatabaseStatus = "Connecting…";
        DatabaseStatus = await database.CanConnectAsync() ? "Connected to database" : "Could not reach the database";
    }
}
