namespace NewsPage.Models.entities
{
    public class FavoriteTopics
    {
        public Guid Id { get; set; }
        public Guid UserId{get; set;}
        public Guid TopicId{get; set;}
    }
}
