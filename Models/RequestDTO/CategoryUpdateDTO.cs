namespace NewsPage.Models.RequestDTO
{
    public class CategoryUpdateDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid TopicId { get; set; }
    }
}
