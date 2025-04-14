using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Models.entities;
using NewsPage.repositories.interfaces;

namespace NewsPage.repositories
{
    public class ArticleVisitRepository : IArticleVisitRepository
    {
        private readonly ApplicationDbContext _context;

        public ArticleVisitRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<ArticleVisit?> CreateAsync(ArticleVisit articleVisit)
        {
            // check trung lap
            if (articleVisit.UserAccountId != null)
            {
                var existingVisit = await _context.ArticleVisits
                                    .FirstOrDefaultAsync(a => a.UserAccountId == articleVisit.UserAccountId
                                   && a.ArticleId == articleVisit.ArticleId);

                if (existingVisit != null)
                {
                    return null;
                }
            }

            articleVisit.Id = Guid.NewGuid();

            await _context.ArticleVisits.AddAsync(articleVisit);
            await _context.SaveChangesAsync();


            return await _context.ArticleVisits
                .FirstOrDefaultAsync(a => a.Id == articleVisit.Id);
        }
        public async Task<int> CountViewsByArticleIdAsync(Guid articleId)
        {
            return await _context.ArticleVisits
                .Where(v => v.ArticleId == articleId)
                .CountAsync();
        }

    }
}
