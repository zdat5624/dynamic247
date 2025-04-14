namespace NewsPage.Models.RequestDTO
{
    public class ArticleCommentStatsRequestDTO
    {
        public DateTime PublishStartDate { get; set; }
        public DateTime PublishEndDate { get; set; }
        public DateTime CommentStartDate { get; set; }
        public DateTime CommentEndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
