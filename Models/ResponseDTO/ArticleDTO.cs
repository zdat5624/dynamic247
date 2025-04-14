using NewsPage.Enums;
using NewsPage.Models.entities;

namespace NewsPage.Models.ResponseDTO
{
    public class ArticleDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Thumbnail { get; set; }
        public required string Content { get; set; }
        public required ArticleStatus Status { get; set; }
        public DateTime? PublishedAt { get; set; }
        public DateTime? UpdateAt { get; set; }
        public required DateTime CreateAt { get; set; }
        public CategoryDTO? Category { get; set; }
        public UserDetails? UserDetails { get; set; }
        public string? UserAccountEmail { get; set; }
    }
}
