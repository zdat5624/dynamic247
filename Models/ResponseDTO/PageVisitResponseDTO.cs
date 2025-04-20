
namespace NewsPage.Models.ResponseDTO;
public class PageVisitResponseDTO{
    public Dictionary<string, int> DailyStats { get; set; }
    public Dictionary<string, int> MonthlyStats { get; set; }
    
    public Dictionary<string, int> YearlyStats { get; set; }
}