using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;
namespace NewsPage.repositories.interfaces;
public interface IReadingFrequencyRepository
{
    Task IncrementReadingCountAsync(Guid userId);
    Task<List<ReadingFrequency>> GetUserReadingFrequencyAsync(Guid userId);
    Task<(IEnumerable<ReadingFrequency> Items, int TotalCount)> GetAllUserReadingFrequencyPaginatedAsync(int pageNumber, int pageSize);
}