using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;

namespace NewsPage.repositories.interfaces
{
    public interface IArticleStorageRepository
    {
        Task<ArticleStorage?> CreateAsync(ArticleStorage articleVisit);
        Task<PaginatedResponseDTO<ArticleStorage>> GetArticleStorageOfUser(Guid userAccountId, int pageNumber, int pageSize);
        Task DeleteAsync(Guid id);
        Task<bool> IsArticleStoredByUser(Guid userAccountId, Guid articleId);
        Task<ArticleStorage?> GetAsync(Guid articleStorageId, Guid userAccountId);

    }
}
