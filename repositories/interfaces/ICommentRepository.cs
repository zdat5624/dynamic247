using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;

namespace NewsPage.repositories.interfaces
{
    public interface ICommentRepository
    {
        Task<Comment> CreateAsync(Comment comment);
        Task<Comment?> GetByIdAsync(Guid id);
        Task<IEnumerable<Comment>> GetByArticleIdAsync(Guid articleId);
        Task DeleteAsync(Comment comment);
        Task<Comment> UpdateAsync(Comment comment);

        Task<PaginatedResponseDTO<Comment>> GetCommentsByArticlePaginatedAsync(
            Guid articleId, int pageNumber, int pageSize, bool descending);
    }
}
