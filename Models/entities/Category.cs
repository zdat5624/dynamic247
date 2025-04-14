namespace NewsPage.Models.entities
{
    public class Category
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }
        public Guid TopicId { get; set; }

        public Topic? Topic { get; set; }
    }
}
