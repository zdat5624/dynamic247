using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.repositories
{
    public class FavoriteTopicRepository : IFavoriteTopicRepository
    {
        private readonly ApplicationDbContext _context;

        public FavoriteTopicRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<FavoriteTopics> AddFavoriteTopicAsync(FavoriteTopics favoriteTopic)
        {
            // Check if this combination already exists
            var existingFavorite = await _context.FavoriteTopics
                .FirstOrDefaultAsync(ft => ft.UserId == favoriteTopic.UserId && ft.TopicId == favoriteTopic.TopicId);
            
            if (existingFavorite != null)
            {
                return existingFavorite; // Return existing if already in favorites
            }

            _context.FavoriteTopics.Add(favoriteTopic);
            await _context.SaveChangesAsync();
            return favoriteTopic;
        }

        public async Task<FavoriteTopics> GetFavoriteTopicByIdAsync(Guid id)
        {
            return await _context.FavoriteTopics
                .FirstOrDefaultAsync(ft => ft.Id == id);
        }

        public async Task<List<FavoriteTopics>> GetFavoriteTopicsByUserIdAsync(Guid userId)
        {
            return await _context.FavoriteTopics
                .Where(ft => ft.UserId == userId)
                .ToListAsync();
        }

        public async Task<bool> DeleteFavoriteTopicAsync(Guid id)
        {
            var favoriteTopic = await _context.FavoriteTopics.FindAsync(id);
            if (favoriteTopic == null)
            {
                return false;
            }

            _context.FavoriteTopics.Remove(favoriteTopic);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteFavoriteTopicByUserAndTopicAsync(Guid userId, Guid topicId)
        {
            var favoriteTopic = await _context.FavoriteTopics
                .FirstOrDefaultAsync(ft => ft.UserId == userId && ft.TopicId == topicId);
            
            if (favoriteTopic == null)
            {
                return false;
            }

            _context.FavoriteTopics.Remove(favoriteTopic);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<PaginatedResponseDTO<FavoriteTopics>> GetPaginatedFavoriteTopicsAsync(
            int pageNumber,
            int pageSize,
            Guid? userId,
            Guid? topicId)
        {
            var query = _context.FavoriteTopics.AsQueryable();

            // Apply filters
            if (userId.HasValue)
            {
                query = query.Where(ft => ft.UserId == userId.Value);
            }

            if (topicId.HasValue)
            {
                query = query.Where(ft => ft.TopicId == topicId.Value);
            }

            // Include related entities if needed
            // query = query.Include(ft => ft.Topic);  // Assuming you have a navigation property to Topic

            // Calculate total count for pagination
            var totalCount = await query.CountAsync();

            // Apply pagination
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            // Create and return the paginated response
             return new PaginatedResponseDTO<FavoriteTopics>(items, pageNumber, pageSize, totalCount);
        }

        public async Task<IEnumerable<object>> GetUsersByTopicIdAsync(Guid topicId)
        {
            return await _context.FavoriteTopics.Where(f => f.TopicId == topicId).ToListAsync();
        }
    }
}