using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Enums;
using NewsPage.Models.entities;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.Repositories
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Article?> CreateAsync(Article article)
        {
            article.Id = Guid.NewGuid();
            article.PublishedAt = null;
            article.UpdateAt = null;

            await _context.Articles.AddAsync(article);
            await _context.SaveChangesAsync();


            return await _context.Articles
                .Include(a => a.UserAccounts)
                .Include(a => a.Category)
                    .ThenInclude(c => c.Topic)
                .FirstOrDefaultAsync(a => a.Id == article.Id);
        }



        public async Task<Article?> GetArticleWithCategoryAsync(Guid id)
        {
            return await _context.Articles
                .Include(a => a.Category)
                    .ThenInclude(c => c.Topic)
                .Include(a => a.UserAccounts)
                .FirstOrDefaultAsync(a => a.Id == id);
        }

        public async Task<Article?> GetByIdAsync(Guid id)
        {
            return await _context.Articles.FindAsync(id);
        }

        public async Task<Article?> UpdateAsync(Article article)
        {
            article.UpdateAt = DateTime.Now;
            _context.Articles.Update(article);
            await _context.SaveChangesAsync();

            return await _context.Articles
                .Include(a => a.UserAccounts)
                .Include(a => a.Category)
                    .ThenInclude(c => c.Topic)
                .FirstOrDefaultAsync(a => a.Id == article.Id);
        }


        public async Task DeleteAsync(Article article)
        {
            var comments = await _context.Comments
            .Where(c => c.ArticleId == article.Id)
            .ToListAsync();

            // Xóa tất cả Comment
            if (comments.Count > 0)
            {
                _context.Comments.RemoveRange(comments);
            }

            // Xóa  ArticleVisit
            var articleVisits = await _context.ArticleVisits
                .Where(c => c.ArticleId == article.Id)
                .ToListAsync();

            if (articleVisits.Count > 0)
            {
                _context.ArticleVisits.RemoveRange(articleVisits);
            }

            var articleStorages = await _context.ArticleStorages
                           .Where(c => c.ArticleId == article.Id)
                           .ToListAsync();
            if (articleStorages.Count > 0)
            {
                _context.ArticleStorages.RemoveRange(articleStorages);
            }

            _context.Articles.Remove(article);

            await _context.SaveChangesAsync();
        }

        public async Task<PaginatedResponseDTO<Article>> GetPaginatedArticlesAsync(
            int pageNumber,
            int pageSize,
            string? searchTerm,
            Guid? categoryId,
            Guid? userAccountId,
            DateTime? publishedAt,
            ArticleStatus? status,
            Guid? topicId,
            string? sortBy,
            string? sortOrder)
        {
            var query = _context.Articles
                .Include(a => a.Category)
                    .ThenInclude(c => c.Topic)
                .Include(a => a.UserAccounts)
                .AsQueryable();

            // Tìm kiếm theo Title hoặc Content
            if (!string.IsNullOrEmpty(searchTerm))
            {
                query = query.Where(a => a.Title.Contains(searchTerm) || a.Content.Contains(searchTerm));
            }

            // Lọc theo CategoryId
            if (categoryId != null)
            {
                query = query.Where(a => a.CategoryId == categoryId.Value);
            }

            // Lọc theo UserAccountId
            if (userAccountId != null)
            {
                query = query.Where(a => a.UserAccountId == userAccountId.Value);
            }

            // Lọc theo PublishedAt
            if (publishedAt != null)
            {
                var date = publishedAt.Value.Date;
                query = query.Where(a => a.PublishedAt.HasValue && a.PublishedAt.Value.Date == date);
            }

            // Lọc theo Status
            if (status != null)
            {
                query = query.Where(a => a.Status == status.Value);
            }

            // Lọc theo TopicId
            if (topicId != null)
            {
                query = query.Where(a => a.Category.TopicId == topicId.Value);
            }

            // Xử lý sắp xếp
            sortBy = sortBy?.ToLower() ?? "publishedat"; // Mặc định 
            sortOrder = sortOrder?.ToLower() ?? "desc";  // Mặc định

            switch (sortBy)
            {
                case "title":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.Title) : query.OrderByDescending(a => a.Title);
                    break;
                case "publishedat":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.PublishedAt) : query.OrderByDescending(a => a.PublishedAt);
                    break;
                case "updateat":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.UpdateAt) : query.OrderByDescending(a => a.UpdateAt);
                    break;
                case "createat":
                    query = sortOrder == "asc" ? query.OrderBy(a => a.CreateAt) : query.OrderByDescending(a => a.CreateAt);
                    break;
                default:
                    query = query.OrderByDescending(a => a.PublishedAt); // default
                    break;
            }

            var totalCount = await query.CountAsync();

            // Phân trang
            var articles = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<Article>(articles, totalCount, pageNumber, pageSize);
        }

        public async Task<PaginatedResponseDTO<UserPostStatsDTO>> GetUserPostStats(DateTime startDate, DateTime endDate, int pageNumber, int pageSize)
        {

            var query = _context.Articles
                .Where(a => a.PublishedAt >= startDate && a.PublishedAt <= endDate)
                .GroupBy(a => a.UserAccountId)
                .Select(g => new UserPostStatsDTO
                {
                    UserAccountId = g.Key,
                    PostCount = g.Count()
                });


            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.PostCount)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<UserPostStatsDTO>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<PaginatedResponseDTO<UserArticleViewStatsDTO>> GetUserArticleViewStats(ArticleViewStatsRequestDTO request)
        {
            var query = _context.ArticleVisits
                .Where(v =>
                    v.VisitTime >= request.ViewStartDate &&
                    v.VisitTime <= request.ViewEndDate &&
                    v.Article != null &&
                    v.Article.Status == ArticleStatus.PUBLISHED &&
                    v.Article.PublishedAt >= request.PublishStartDate &&
                    v.Article.PublishedAt <= request.PublishEndDate)
                .GroupBy(v => v.Article.UserAccountId)
                .Select(g => new UserArticleViewStatsDTO
                {
                    UserAccountId = g.Key,
                    TotalViews = g.Count()
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.TotalViews)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<UserArticleViewStatsDTO>(items, totalCount, request.PageNumber, request.PageSize);
        }

        public async Task<PaginatedResponseDTO<ArticleCommentStatsDTO>> GetUserArticleCommentStats(ArticleCommentStatsRequestDTO request)
        {
            var query = _context.Comments
                .Where(c =>
                    c.CreatedAt >= request.CommentStartDate &&
                    c.CreatedAt <= request.CommentEndDate &&
                    c.Article != null &&
                    c.Article.Status == ArticleStatus.PUBLISHED &&
                    c.Article.PublishedAt >= request.PublishStartDate &&
                    c.Article.PublishedAt <= request.PublishEndDate)
                .GroupBy(c => c.Article.Id)
                .Select(g => new ArticleCommentStatsDTO
                {
                    ArticleId = g.Key,
                    CommentCount = g.Count()
                });

            var totalCount = await query.CountAsync();

            var items = await query
                .OrderByDescending(x => x.CommentCount)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<ArticleCommentStatsDTO>(items, totalCount, request.PageNumber, request.PageSize);
        }



    }
}
