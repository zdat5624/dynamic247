using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.Repositories
{

    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }



        public async Task<Category?> GetByIdAsync(Guid id)
        {
            return await _context.Categories.Include(c => c.Topic).FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<Category?> AddAsync(Category category)
        {
            await _context.Categories.AddAsync(category);
            await _context.SaveChangesAsync();

            return await _context.Categories.Include(c => c.Topic)
                                            .FirstOrDefaultAsync(c => c.Id == category.Id);
        }



        public async Task<Category?> UpdateAsync(Category category)
        {
            var existingCategory = await _context.Categories.FirstOrDefaultAsync(c => c.Id == category.Id);
            if (existingCategory != null)
            {
                existingCategory.Name = category.Name;
                existingCategory.TopicId = category.TopicId;

                await _context.SaveChangesAsync();

                return await _context.Categories.Include(c => c.Topic)
                                                .FirstOrDefaultAsync(c => c.Id == category.Id);
            }
            return null;
        }


        public async Task DeleteAsync(Guid id)
        {
            var category = await _context.Categories
                .Include(c => c.Topic)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                return;
            }

            // Lấy tất cả Article
            var articles = await _context.Articles
                .Where(a => a.CategoryId == id)
                .ToListAsync();

            // Duyệt Article
            foreach (var article in articles)
            {
                // Xóa tất cả Comment
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
                var articleStorages = await _context.ArticleStorages
                            .Where(c => c.ArticleId == article.Id)
                            .ToListAsync();
                if (articleStorages.Count > 0)
                {
                    _context.ArticleStorages.RemoveRange(articleStorages);
                }
                // Xóa Article
                _context.Articles.Remove(article);
            }

            // Xóa Category
            _context.Categories.Remove(category);

            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<Category>> GetCategoriesByTopicIdAsync(Guid topicId)
        {
            return await _context.Categories
                                 .Include(c => c.Topic)
                                 .Where(c => c.TopicId == topicId)
                                 .ToListAsync();
        }


        public async Task<PaginatedResponseDTO<Category>> GetPaginatedCategoriesAsync(
            int pageNumber,
            int pageSize,
            string? searchName = null,
            Guid? topicId = null,
            bool sortByNameAsc = true)
        {
            var query = _context.Categories.Include(c => c.Topic).AsQueryable();

            if (!string.IsNullOrEmpty(searchName))
            {
                query = query.Where(c => c.Name.ToLower().Contains(searchName.ToLower()));
            }

            if (topicId != null)
            {
                query = query.Where(c => c.TopicId == topicId.Value);
            }

            query = sortByNameAsc
                ? query.OrderBy(c => c.Name)
                : query.OrderByDescending(c => c.Name);

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return new PaginatedResponseDTO<Category>(items, totalCount, pageNumber, pageSize);
        }


    }

}
