using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.repositories
{
    public class TopicRepository : ITopicRepository
    {
        private readonly ApplicationDbContext _context;

        public TopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<Topic?> GetTopicByIdAsync(Guid id)
        {
            var topic = await _context.Topics.FindAsync(id);
            return topic;
        }

        public async Task<Topic> AddTopicAsync(Topic topic)
        {
            _context.Topics.Add(topic);
            await _context.SaveChangesAsync();
            return topic;
        }

        public async Task<Topic?> UpdateTopicAsync(Topic updatedTopic)
        {
            var existingTopic = await _context.Topics.FindAsync(updatedTopic.Id);
            if (existingTopic == null) return null;

            existingTopic.Name = updatedTopic.Name;
            await _context.SaveChangesAsync();
            return existingTopic;
        }

        public async Task<bool> DeleteTopicAsync(Guid topicId)
        {
            try
            {
                // Tìm Topic cần xóa
                var topic = await _context.Topics
                    .FirstOrDefaultAsync(t => t.Id == topicId);

                if (topic == null)
                {
                    return false;
                }

                // Lấy tất cả Category
                var categories = await _context.Categories
                    .Where(c => c.TopicId == topicId)
                    .ToListAsync();

                foreach (var category in categories)
                {

                    var articles = await _context.Articles
                        .Where(a => a.CategoryId == category.Id)
                        .ToListAsync();

                    foreach (var article in articles)
                    {
                        // Xóa  Comment
                        var comments = await _context.Comments
                            .Where(c => c.ArticleId == article.Id)
                            .ToListAsync();

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

                        // Xóa Article
                        _context.Articles.Remove(article);
                    }

                    // Xóa Category
                    _context.Categories.Remove(category);
                }

                _context.Topics.Remove(topic);

                await _context.SaveChangesAsync();

                return true;
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        public async Task<PaginatedResponseDTO<Topic>> GetPaginatedTopicsAsync(
            int pageNumber,
            int pageSize,
            string? searchName = null,
            bool sortByNameAsc = true)
        {
            var query = _context.Topics.AsQueryable();

            // search
            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(t => t.Name.ToLower().Contains(searchName.ToLower()));
            }

            // sort
            query = sortByNameAsc
                ? query.OrderBy(t => t.Name)
                : query.OrderByDescending(t => t.Name);

            var totalCount = await query.CountAsync();

            // Phân trang
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<Topic>(items, totalCount, pageNumber, pageSize);
        }



    }
}
