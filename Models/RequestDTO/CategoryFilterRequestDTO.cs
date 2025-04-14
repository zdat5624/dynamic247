namespace NewsPage.Models.RequestDTO
{
    public class CategoryFilterRequestDTO
    {
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public string? SearchName { get; set; }
        public Guid? TopicId { get; set; }
        public bool SortByNameAsc { get; set; } = true;
    }
}
