using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;

namespace NewsPage.repositories.interfaces
{
    public interface IFavoriteTopicRepository
    {
        Task<FavoriteTopics> AddFavoriteTopicAsync(FavoriteTopics favoriteTopic);
        Task<FavoriteTopics> GetFavoriteTopicByIdAsync(Guid id);
        Task<List<FavoriteTopics>> GetFavoriteTopicsByUserIdAsync(Guid userId);
        Task<bool> DeleteFavoriteTopicAsync(Guid id);
        Task<bool> DeleteFavoriteTopicByUserAndTopicAsync(Guid userId, Guid topicId);
        Task<PaginatedResponseDTO<FavoriteTopics>> GetPaginatedFavoriteTopicsAsync(
            int pageNumber,
            int pageSize,
            Guid? userId,
            Guid? topicId);
        Task<IEnumerable<object>> GetUsersByTopicIdAsync(Guid topicId);
    }
}
