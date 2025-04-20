using NewsPage.Models.entities;
using NewsPage.Models.ResponseDTO;

namespace NewsPage.repositories.interfaces
{
    public interface IPageVisitorRepository
    {
        Task<PageVisitResponseDTO> GetTotalViews();
    }
}
