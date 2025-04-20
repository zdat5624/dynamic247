using Microsoft.EntityFrameworkCore;
using NewsPage.data;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.repositories{
    public class PageVisitorRepository : IPageVisitorRepository
    {
        private readonly ApplicationDbContext _context;

        public PageVisitorRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task <PageVisitResponseDTO>GetTotalViews()
        {
            // Get all page visit records
        var allVisits = await _context.PageVisitors.ToListAsync();
        
        if (allVisits == null || !allVisits.Any())
        {
            return null;
        }

        // Calculate total visits
        int totalVisits = allVisits.Sum(v => v.AccessCount);

        // Group by day for daily sums
        var dailySums = allVisits
            .GroupBy(v => v.CreateAt.Date)
            .ToDictionary(
                g => g.Key.ToString("yyyy-MM-dd"),
                g => g.Sum(v => v.AccessCount)
            );
        // Group by year for yearly sums
        var monthlySums = allVisits
            .GroupBy(v => v.CreateAt.Month)
            .ToDictionary(
                g => g.Key.ToString(),
                g => g.Sum(v => v.AccessCount)
            );

        // Group by year for yearly sums
        var yearlySums = allVisits
            .GroupBy(v => v.CreateAt.Year)
            .ToDictionary(
                g => g.Key.ToString(),
                g => g.Sum(v => v.AccessCount)
            );
        return new PageVisitResponseDTO {DailyStats =  dailySums, MonthlyStats =  monthlySums, YearlyStats = yearlySums};
        }
    }
}