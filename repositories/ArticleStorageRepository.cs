using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.repositories
{
    public class ArticleStorageRepository : IArticleStorageRepository
    {
        private readonly ApplicationDbContext _context;
        public ArticleStorageRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<ArticleStorage?> CreateAsync(ArticleStorage articleStorage)
        {
            var existingStorage = await _context.ArticleStorages
                .FirstOrDefaultAsync(a => a.UserAccountId == articleStorage.UserAccountId
                                       && a.ArticleId == articleStorage.ArticleId);

            if (existingStorage != null)
            {
                return null;
            }

            articleStorage.Id = Guid.NewGuid();
            articleStorage.CreateAt = DateTime.UtcNow;

            await _context.ArticleStorages.AddAsync(articleStorage);
            await _context.SaveChangesAsync();


            return await _context.ArticleStorages
                .FirstOrDefaultAsync(a => a.Id == articleStorage.Id);
        }

        public async Task<ArticleStorage?> GetAsync(Guid articleStorageId, Guid userAccountId)
        {
            var existingStorage = await _context.ArticleStorages
                .FirstOrDefaultAsync(a => a.Id == articleStorageId && a.UserAccountId == userAccountId);


            return existingStorage;
        }

        public async Task<PaginatedResponseDTO<ArticleStorage>> GetArticleStorageOfUser(Guid userAccountId, int pageNumber, int pageSize)
        {
            var totalCount = await _context.ArticleStorages.CountAsync();
            var items = await _context.ArticleStorages
                .Include(a => a.Article)
                .OrderByDescending(x => x.CreateAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return new PaginatedResponseDTO<ArticleStorage>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<bool> IsArticleStoredByUser(Guid userAccountId, Guid articleId)
        {
            return await _context.ArticleStorages
                .AnyAsync(a => a.UserAccountId == userAccountId && a.ArticleId == articleId);
        }

        public async Task DeleteAsync(Guid id)
        {
            var existingStorage = await _context.ArticleStorages
                .FirstOrDefaultAsync(a => a.Id == id);

            _context.ArticleStorages.Remove(existingStorage);
            await _context.SaveChangesAsync();

        }
    }
}
