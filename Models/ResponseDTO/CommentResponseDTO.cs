namespace NewsPage.Models.ResponseDTO
{
    public class CommentResponseDTO
    {
        public Guid Id { get; set; }
        public string Content { get; set; }
        public bool IsHiden { get; set; }
        public DateTime CreatedAt { get; set; }
        public Guid ArticleId { get; set; }
        public Guid UserAccountId { get; set; }
    }
}
