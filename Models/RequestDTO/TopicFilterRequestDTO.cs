namespace NewsPage.Models.RequestDTO
{
    public class TopicFilterRequestDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchName { get; set; }
        public bool SortByNameAsc { get; set; } = true;
    }
}
