using Lms.Application.Models;

namespace Lms.Application.Repositories;

public class LibraryRepository : ILibraryRepository
{
    private readonly List<Library> _libraries = new List<Library>();

    public async Task<IReadOnlyList<Library>> GetAllLibrariesAsync()
    {
        return await Task.FromResult(_libraries.AsReadOnly());
    }

    public async Task<Library> GetLibraryByIdAsync(Guid libraryId)
    {
        var library = _libraries.FirstOrDefault(l => l.Id == libraryId);
        if (library == null)
        {
            throw new KeyNotFoundException($"Library with ID {libraryId} not found.");
        }
        return await Task.FromResult(library);
    }

    public async Task<Library> CreateLibraryAsync(Library library)
    {
        library.Id = Guid.NewGuid();
        _libraries.Add(library);
        return await Task.FromResult(library);
    }

    public async Task<Library> UpdateLibraryAsync(Library library)
    {
        var existingLibrary = _libraries.FirstOrDefault(l => l.Id == library.Id);

        if (existingLibrary == null)
        {
            throw new KeyNotFoundException($"Library with ID {library.Id} not found.");
        }

        existingLibrary.Name = library.Name;
        existingLibrary.Users = library.Users;
        existingLibrary.Books = library.Books;

        return await Task.FromResult(existingLibrary);
    }
}