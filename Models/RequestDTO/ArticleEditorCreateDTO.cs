using NewsPage.Enums;

namespace NewsPage.Models.RequestDTO
{
    public class ArticleEditorCreateDTO
    {
        public required string Title { get; set; }
        public required string Thumbnail { get; set; }
        public required string Content { get; set; }
        public required ArticleStatus Status { get; set; } //  DRAFT Bản nháp hoặc  PENDING Đang chờ Admin duyệt
        public required Boolean IsShowAuthor { get; set; }
        public Guid CategoryId { get; set; }
    }
}
