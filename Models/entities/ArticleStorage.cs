using System.Text.Json.Serialization;

namespace NewsPage.Models.entities
{
    public class ArticleStorage
    {
        public Guid Id { get; set; }
        public required DateTime CreateAt { get; set; }
        [JsonIgnore]
        public Guid? UserAccountId { get; set; }
        [JsonIgnore]
        public Guid ArticleId { get; set; }
        [JsonIgnore]
        public UserAccounts? UserAccounts { get; set; }

        public Article? Article { get; set; }
    }
}
