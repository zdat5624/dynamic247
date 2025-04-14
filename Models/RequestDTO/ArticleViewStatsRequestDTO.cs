namespace NewsPage.Models.RequestDTO
{
    public class ArticleViewStatsRequestDTO
    {
        public DateTime PublishStartDate { get; set; }
        public DateTime PublishEndDate { get; set; }
        public DateTime ViewStartDate { get; set; }
        public DateTime ViewEndDate { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
