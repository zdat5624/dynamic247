using NewsPage.Models.entities;

namespace NewsPage.repositories.interfaces
{
    public interface IArticleVisitRepository
    {
        Task<ArticleVisit?> CreateAsync(ArticleVisit articleVisit);
        Task<int> CountViewsByArticleIdAsync(Guid articleId);
    }
}
