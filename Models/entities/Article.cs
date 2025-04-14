using NewsPage.Enums;

namespace NewsPage.Models.entities
{
    public class Article
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Thumbnail { get; set; }
        public required string Content { get; set; }
        public required ArticleStatus Status { get; set; }
        public required Boolean IsShowAuthor { get; set; }
        public DateTime? PublishedAt { get; set; } = null;
        public DateTime? UpdateAt { get; set; } = null;
        public required DateTime CreateAt { get; set; }
        public Guid UserAccountId { get; set; }
        public Guid CategoryId { get; set; }
        public UserAccounts? UserAccounts { get; set; }
        public Category? Category { get; set; }
    }
}
