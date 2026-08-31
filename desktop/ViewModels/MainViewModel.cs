using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lms.Application.Models;
using Lms.Application.Services;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Lms.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly ILibraryService _libraryService;

    public ObservableCollection<Book> Books { get; } = new();
    public ObservableCollection<Book> BorrowedBooks { get; } = new();
    public ObservableCollection<User> Users { get; } = new();

    [ObservableProperty]
    private Library? _library;

    [ObservableProperty]
    private User? _selectedUser;

    [ObservableProperty]
    private string _statusMessage = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddBookCommand))]
    private string _newBookTitle = string.Empty;

    [ObservableProperty]
    private string _newBookIsbn = string.Empty;

    [ObservableProperty]
    private string _newAuthorFirstName = string.Empty;

    [ObservableProperty]
    private string _newAuthorLastName = string.Empty;

    public MainViewModel(ILibraryService libraryService)
    {
        _libraryService = libraryService;
    }

    public async Task InitializeAsync()
    {
        Library = await _libraryService.GetOrCreateLibraryAsync("Tech College Library");

        if (Library.Users.Count == 0)
        {
            await SeedSampleDataAsync();
        }

        foreach (var user in Library.Users)
        {
            Users.Add(user);
        }

        SelectedUser = Users.FirstOrDefault();
        await RefreshAsync();
    }

    private async Task SeedSampleDataAsync()
    {
        if (Library is null) return;

        await _libraryService.RegisterUserAsync(Library.Id,
            new User { FirstName = "Tobias", LastName = "Christiansen" });
        await _libraryService.RegisterUserAsync(Library.Id,
            new PremiumUser { FirstName = "Alice", LastName = "Premium" });

        await _libraryService.AddBookAsync(Library.Id, new Book
        {
            Title = "Clean Code",
            ISBN = "978-0132350884",
            Authors = { new Author { Id = Guid.NewGuid(), FirstName = "Robert", LastName = "Martin" } },
        });
        await _libraryService.AddBookAsync(Library.Id, new Book
        {
            Title = "The Pragmatic Programmer",
            ISBN = "978-0135957059",
            Authors = { new Author { Id = Guid.NewGuid(), FirstName = "Andrew", LastName = "Hunt" } },
        });
        await _libraryService.AddBookAsync(Library.Id, new Book
        {
            Title = "C# in Depth",
            ISBN = "978-1617294532",
            Authors = { new Author { Id = Guid.NewGuid(), FirstName = "Jon", LastName = "Skeet" } },
        });
    }

    private async Task RefreshAsync()
    {
        if (Library is null) return;

        Books.Clear();
        foreach (var book in await _libraryService.GetAllBooksAsync(Library.Id))
        {
            Books.Add(book);
        }

        await RefreshBorrowedBooksAsync();
    }

    private async Task RefreshBorrowedBooksAsync()
    {
        BorrowedBooks.Clear();

        if (Library is null || SelectedUser is null) return;

        foreach (var book in await _libraryService.GetBorrowedBooksAsync(Library.Id, SelectedUser.Id))
        {
            BorrowedBooks.Add(book);
        }
    }

    partial void OnSelectedUserChanged(User? value)
    {
        _ = RefreshBorrowedBooksAsync();
    }

    [RelayCommand]
    private async Task LoanBookAsync(Book book)
    {
        if (Library is null || SelectedUser is null) return;

        var success = await _libraryService.BorrowBookAsync(Library.Id, SelectedUser.Id, book.Id);

        StatusMessage = success
            ? $"{SelectedUser.DisplayName} borrowed \"{book.Title}\"."
            : $"Could not borrow \"{book.Title}\" (not available, or borrow limit reached).";

        await RefreshAsync();
    }

    [RelayCommand]
    private async Task ReturnBookAsync(Book book)
    {
        if (Library is null || SelectedUser is null) return;

        var success = await _libraryService.ReturnBookAsync(Library.Id, SelectedUser.Id, book.Id);

        StatusMessage = success
            ? $"{SelectedUser.DisplayName} returned \"{book.Title}\"."
            : $"Could not return \"{book.Title}\".";

        await RefreshAsync();
    }

    private bool CanAddBook() => !string.IsNullOrWhiteSpace(NewBookTitle);

    [RelayCommand(CanExecute = nameof(CanAddBook))]
    private async Task AddBookAsync()
    {
        if (Library is null) return;

        var book = new Book
        {
            Title = NewBookTitle.Trim(),
            ISBN = NewBookIsbn.Trim(),
        };

        if (!string.IsNullOrWhiteSpace(NewAuthorFirstName) || !string.IsNullOrWhiteSpace(NewAuthorLastName))
        {
            book.Authors.Add(new Author
            {
                Id = Guid.NewGuid(),
                FirstName = NewAuthorFirstName.Trim(),
                LastName = NewAuthorLastName.Trim(),
            });
        }

        await _libraryService.AddBookAsync(Library.Id, book);
        StatusMessage = $"Added \"{book.Title}\" to the library.";

        NewBookTitle = string.Empty;
        NewBookIsbn = string.Empty;
        NewAuthorFirstName = string.Empty;
        NewAuthorLastName = string.Empty;

        await RefreshAsync();
    }
}
