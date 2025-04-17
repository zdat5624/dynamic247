// FavoriteTopicCreateDTO class
namespace NewsPage.Models.RequestDTO
{
    public class FavoriteTopicCreateDTO
    {
        public Guid UserId { get; set; }
        public Guid TopicId { get; set; }
    }
}