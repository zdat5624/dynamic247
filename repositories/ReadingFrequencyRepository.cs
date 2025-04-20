using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Models.entities;
using NewsPage.repositories.interfaces;
namespace NewsPage.repositories;
public class ReadingFrequencyRepository : IReadingFrequencyRepository
{
    private readonly ApplicationDbContext _context;
    
    public ReadingFrequencyRepository(ApplicationDbContext context)
    {
        _context = context;
    }
    
    public async Task IncrementReadingCountAsync(Guid userId)
    {
        var readingFrequency = await _context.ReadingFrequencies
            .FirstOrDefaultAsync(rf => rf.UserId == userId);
            
        if (readingFrequency == null)
        {
            readingFrequency = new ReadingFrequency
            {
                UserId = userId,
                CreateAt = DateTime.UtcNow,
                ReadingCount = 1
            };
            _context.ReadingFrequencies.Add(readingFrequency);
        }
        else
        {
            readingFrequency.ReadingCount++;
        }
        
        await _context.SaveChangesAsync();
    }
    
    public async Task<List<ReadingFrequency>> GetUserReadingFrequencyAsync(Guid userId)
    {
        return await _context.ReadingFrequencies
            .Where(rf => rf.UserId == userId).ToListAsync();
    }
    public async Task<(IEnumerable<ReadingFrequency> Items, int TotalCount)> GetAllUserReadingFrequencyPaginatedAsync(int pageNumber, int pageSize)
    {
        // Calculate how many items to skip
        int itemsToSkip = (pageNumber - 1) * pageSize;
        
        // Get total count for pagination info
        int totalCount = await _context.ReadingFrequencies.CountAsync();
        
        // Get the paginated items
        var items = await _context.ReadingFrequencies
            .OrderByDescending(rf => rf.ReadingCount)
            .Skip(itemsToSkip)
            .Take(pageSize)
            .ToListAsync();
        
        return (items, totalCount);
    }
}