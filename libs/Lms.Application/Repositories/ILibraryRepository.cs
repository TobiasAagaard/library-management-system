using Lms.Application.Models;
namespace Lms.Application.Repositories;

public interface ILibraryRepository
{
    Task<IReadOnlyList<Library>> GetAllLibrariesAsync();
    Task<Library> GetLibraryByIdAsync(Guid libraryId);
    Task<Library> CreateLibraryAsync(Library library);
    Task<Library> UpdateLibraryAsync(Library library);
} 
