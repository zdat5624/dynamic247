using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;

namespace NewsPage.repositories.interfaces
{
    public interface ITopicRepository
    {
        Task<Topic?> GetTopicByIdAsync(Guid id);
        Task<Topic> AddTopicAsync(Topic topic);
        Task<Topic?> UpdateTopicAsync(Topic updatedTopic);
        Task<Boolean> DeleteTopicAsync(Guid topicId);

        Task<PaginatedResponseDTO<Topic>> GetPaginatedTopicsAsync(
            int pageNumber,
            int pageSize,
            string? searchName = null,
            bool sortByNameAsc = true);
    }
}
