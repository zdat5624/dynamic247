using NewsPage.Models.entities;

namespace NewsPage.Models.ResponseDTO
{
    public class CategoryDTO
    {
        public Guid Id { get; set; }
        public required string Name { get; set; }

        public Topic? Topic { get; set; }
    }
}
