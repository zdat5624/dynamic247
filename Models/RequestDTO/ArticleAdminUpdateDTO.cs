namespace NewsPage.Models.RequestDTO
{
    public class ArticleAdminUpdateDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Thumbnail { get; set; }
        public required string Content { get; set; }
        public Guid? UserAccountId { get; set; }
        public Guid CategoryId { get; set; }

    }
}
