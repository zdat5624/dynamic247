namespace NewsPage.Models.ResponseDTO
{
    public class ArticleCommentStatsDTO
    {
        public Guid ArticleId { get; set; }
        public long CommentCount { get; set; }
    }
}
