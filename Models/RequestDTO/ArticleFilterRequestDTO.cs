using NewsPage.Enums;

namespace NewsPage.Models.RequestDTO
{
    public class ArticleFilterRequestDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchTerm { get; set; }
        public Guid? CategoryId { get; set; }
        public Guid? UserAccountId { get; set; }
        public DateTime? PublishedAt { get; set; }
        public ArticleStatus? Status { get; set; }
        public Guid? TopicId { get; set; }
        public string? SortBy { get; set; } = "PublishedAt";
        public string? SortOrder { get; set; } = "desc";
    }
}
