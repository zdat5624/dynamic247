namespace NewsPage.Models.RequestDTO
{
    public class CategoryCreateDTO
    {
        public required string Name { get; set; }
        public Guid TopicId { get; set; }
    }
}
