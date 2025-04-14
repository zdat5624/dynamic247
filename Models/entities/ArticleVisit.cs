using System.Text.Json.Serialization;

namespace NewsPage.Models.entities
{
    public class ArticleVisit
    {
        public Guid Id { get; set; }
        public required DateTime VisitTime { get; set; }
        public Guid? UserAccountId { get; set; }
        public required Guid ArticleId { get; set; }
        [JsonIgnore]
        public UserAccounts? UserAccounts { get; set; }
        [JsonIgnore]
        public Article? Article { get; set; }

    }
}
