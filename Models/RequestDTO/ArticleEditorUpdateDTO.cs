namespace NewsPage.Models.RequestDTO
{
    public class ArticleEditorUpdateDTO
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public required string Thumbnail { get; set; }
        public required string Content { get; set; }
        public required Boolean IsShowAuthor { get; set; }
        public Guid CategoryId { get; set; }
    }
}
