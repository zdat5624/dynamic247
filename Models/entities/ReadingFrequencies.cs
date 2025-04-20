using System.ComponentModel.DataAnnotations;

namespace NewsPage.Models.entities
{
    public class ReadingFrequency
    {
        [Key]
        public Guid Id { get; set; }
        public Guid? UserId { get; set; }
        public DateTime CreateAt { get; set; }
        public int ReadingCount { get; set; }

        // // CountdownEvent by second
        // public int TotalReadingTime { get; set; }
    }
}
