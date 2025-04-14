using Microsoft.AspNetCore.Mvc;
using NewsPage.Models.entities;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;
using System.Security.Claims;

namespace NewsPage.Controllers.Articles
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticleVisitController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IArticleVisitRepository _articleVisitRepository;

        public ArticleVisitController(IUserAccountRepository userAccountRepository, IArticleVisitRepository articleVisitRepository)
        {
            _userAccountRepository = userAccountRepository;
            _articleVisitRepository = articleVisitRepository;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ArticleVisit>>> CreateArticleViSit([FromBody] ArticleVisitCreateDTO articleVisitCreateDTO)
        {
            UserAccounts? userAccount = null;
            // Email
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail != null)
            {
                userAccount = await _userAccountRepository.GetByEmail(userEmail);
                if (userAccount == null) return NotFound(new ApiResponse<ArticleDTO>(404, "Người dùng không tồn tại."));
            }


            ArticleVisit articleVisit = new ArticleVisit
            {
                ArticleId = articleVisitCreateDTO.ArticleId,
                VisitTime = DateTime.UtcNow,
                UserAccountId = userAccount?.Id
            };

            var addedArticleVisit = await _articleVisitRepository.CreateAsync(articleVisit);

            if (addedArticleVisit == null)
            {
                return StatusCode(400, new ApiResponse<ArticleVisit>(400, "Ghi nhận truy cập bài viết thất bại: lượt ghi nhận bị trùng lặp", null));
            }


            return StatusCode(201, new ApiResponse<ArticleVisit>(201, "Ghi nhận truy cập bài viết thành công", addedArticleVisit));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalViewByArticleId(Guid id)
        {


            var totalViews = await _articleVisitRepository.CountViewsByArticleIdAsync(id);

            return Ok(new ApiResponse<int>(200, $"Tổng số lượt xem cho bài viết", totalViews));
        }
    }
}
