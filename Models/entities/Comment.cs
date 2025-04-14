namespace NewsPage.Models.entities
{
    public class Comment
    {
        public Guid Id { get; set; }
        public required string Content { get; set; }
        public Boolean IsHiden { get; set; } = false;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public Guid ArticleId { get; set; }

        public Guid UserAccountId { get; set; }
        public UserAccounts UserAccounts { get; set; }
        public Article Article { get; set; }
    }
}
