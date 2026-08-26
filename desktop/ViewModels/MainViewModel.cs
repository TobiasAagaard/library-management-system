using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Lms.Application.Models;
using Lms.Application.Repositories;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Lms.Desktop.ViewModels;

public partial class MainViewModel : ViewModelBase
{
    private readonly IBookRepository _bookRepository;
    private readonly ILibraryRepository _libraryRepository;

    public ObservableCollection<Book> Books { get; } = new();

    [ObservableProperty]
    private Library? _library;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(AddBookCommand))]
    private string _newBookTitle = string.Empty;

    [ObservableProperty]
    private string _newBookIsbn = string.Empty;

    [ObservableProperty]
    private string _newAuthorFirstName = string.Empty;

    [ObservableProperty]
    private string _newAuthorLastName = string.Empty;

    public MainViewModel(IBookRepository bookRepository, ILibraryRepository libraryRepository)
    {
        _bookRepository = bookRepository;
        _libraryRepository = libraryRepository;
    }

    public async Task InitializeAsync()
    {
        var libraries = await _libraryRepository.GetAllLibrariesAsync();

        Library = libraries.Count > 0
            ? libraries[0]
            : await _libraryRepository.CreateLibraryAsync(new Library { Name = "Tech College Library" });

        foreach (var book in await _bookRepository.GetAllBooksAsync())
        {
            Books.Add(book);
        }
    }

    private bool CanAddBook() => !string.IsNullOrWhiteSpace(NewBookTitle);

    [RelayCommand(CanExecute = nameof(CanAddBook))]
    private async Task AddBookAsync()
    {
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

        var created = await _bookRepository.CreateBookAsync(book);
        Books.Add(created);

        if (Library != null)
        {
            Library.Books.Add(created);
            await _libraryRepository.UpdateLibraryAsync(Library);
        }

        NewBookTitle = string.Empty;
        NewBookIsbn = string.Empty;
        NewAuthorFirstName = string.Empty;
        NewAuthorLastName = string.Empty;
    }
}
