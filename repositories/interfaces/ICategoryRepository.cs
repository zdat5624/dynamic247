using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;

namespace NewsPage.repositories.interfaces
{
    public interface ICategoryRepository
    {
        Task<Category?> GetByIdAsync(Guid id);
        Task<Category?> AddAsync(Category category);
        Task<Category?> UpdateAsync(Category category);
        Task DeleteAsync(Guid id);

        Task<IEnumerable<Category>> GetCategoriesByTopicIdAsync(Guid topicId);

        Task<PaginatedResponseDTO<Category>> GetPaginatedCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? searchName = null,
            Guid? topicId = null,
            bool sortByNameAsc = true);

    }
}
