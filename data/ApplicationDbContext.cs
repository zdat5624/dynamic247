using Microsoft.EntityFrameworkCore;
using NewsPage.Models.entities;

namespace NewsPage.data
{
    public class ApplicationDbContext : DbContext
    {
        private readonly IConfiguration _configuration;

        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options, IConfiguration configuration)
            : base(options)
        {
            _configuration = configuration;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            var adminEmail = _configuration["AdminSettings:Email"];
            var adminPasswordHash = _configuration["AdminSettings:PasswordHash"];

            //create  admin account
            modelBuilder.Entity<UserAccounts>().HasData(
                new UserAccounts
                {
                    Id = new Guid("11111111-1111-1111-1111-111111111111"),
                    Email = adminEmail,
                    Password = adminPasswordHash, // Lấy từ appsettings.json
                    Role = "Admin",
                    CreatedAt = new DateTime(2024, 3, 20, 0, 0, 0, DateTimeKind.Utc),
                    Status = "Enable",
                    IsVerified = true,
                }
            );



            //config unique email
            modelBuilder.Entity<UserAccounts>()
                .HasIndex(u => u.Email)
                .IsUnique();

            //define relation 1 to 1 in UserAccounts and UserDetails 
            modelBuilder.Entity<UserAccounts>()
                .HasOne<UserDetails>()
                .WithOne()
                .HasForeignKey<UserDetails>(ud => ud.UserAccountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);


            //  UserAccount Article (1-N)
            modelBuilder.Entity<Article>()
                .HasOne(a => a.UserAccounts)
                .WithMany()
                .HasForeignKey(a => a.UserAccountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<Category>()
                .HasOne(c => c.Topic)
                .WithMany()
                .HasForeignKey(c => c.TopicId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            // Category  Article (1-N)
            modelBuilder.Entity<Article>()
                .HasOne(a => a.Category)
                .WithMany()
                .HasForeignKey(a => a.CategoryId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //  Article Comment (1-N)
            modelBuilder.Entity<Comment>()
                .HasOne(co => co.Article)
                .WithMany()
                .HasForeignKey(c => c.ArticleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // UserAccount Comment (1-N)
            modelBuilder.Entity<Comment>()
                .HasOne(co => co.UserAccounts)
                .WithMany()
                .HasForeignKey(c => c.UserAccountId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Cascade);

            //  Article ArticleVisit (1-N)
            modelBuilder.Entity<ArticleVisit>()
                .HasOne(co => co.Article)
                .WithMany()
                .HasForeignKey(c => c.ArticleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // UserAccount ArticleVisit (1-N)
            modelBuilder.Entity<ArticleVisit>()
                .HasOne(co => co.UserAccounts)
                .WithMany()
                .HasForeignKey(c => c.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            //  Article ArticleStorage (1-N)
            modelBuilder.Entity<ArticleStorage>()
                .HasOne(co => co.Article)
                .WithMany()
                .HasForeignKey(c => c.ArticleId)
                .IsRequired()
                .OnDelete(DeleteBehavior.Restrict);

            // UserAccount ArticleStorage (1-N)
            modelBuilder.Entity<ArticleStorage>()
                .HasOne(co => co.UserAccounts)
                .WithMany()
                .HasForeignKey(c => c.UserAccountId)
                .OnDelete(DeleteBehavior.Cascade);

            //  enum ArticleStatus
            modelBuilder.Entity<Article>()
                .Property(a => a.Status)
                .HasConversion<string>();

            modelBuilder.Entity<Article>()
                .Property(a => a.Content)
                .HasColumnType("NVARCHAR(MAX)");
            modelBuilder.Entity<FavoriteTopics>()
                .HasOne<UserAccounts>()
                .WithMany()
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<FavoriteTopics>()
                .HasOne<Topic>()
                .WithMany()
                .HasForeignKey(t => t.TopicId)
                .OnDelete(DeleteBehavior.Cascade);

            modelBuilder.Entity<PageVisitor>()
                .HasOne<UserAccounts>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<ReadingFrequency>()
                .HasOne<UserAccounts>()
                .WithMany()
                .HasForeignKey(p => p.UserId)
                .OnDelete(DeleteBehavior.Cascade);
            base.OnModelCreating(modelBuilder);



        }

        public DbSet<UserAccounts> UserAccounts { get; set; }
        public DbSet<UserDetails> UserDetails { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<Topic> Topics { get; set; }
        public DbSet<Article> Articles { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<ArticleVisit> ArticleVisits { get; set; }
        public DbSet<FavoriteTopics> FavoriteTopics { get; set; }
        public DbSet<PageVisitor> PageVisitors { get; set; }
        public DbSet<ArticleStorage> ArticleStorages { get; set; }
        public DbSet<ReadingFrequency> ReadingFrequencies { get; set; }




    }
}
