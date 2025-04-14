namespace NewsPage.Models.RequestDTO
{
    public class CommentCreateDTO
    {
        public required string Content { get; set; }

        public Guid ArticleId { get; set; }

    }
}
