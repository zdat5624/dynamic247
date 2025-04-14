using NewsPage.Enums;

namespace NewsPage.Models.RequestDTO
{
    public class ArticleStatusUpdateDTO
    {
        public required ArticleStatus Status { get; set; }
    }
}
