using Microsoft.AspNetCore.Mvc;
using NewsPage.Models.entities;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;
using NLog;
using System.Security.Claims;

namespace NewsPage.Controllers.Articles
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticleVisitController : ControllerBase
    {
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IArticleVisitRepository _articleVisitRepository;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ArticleVisitController(IUserAccountRepository userAccountRepository, IArticleVisitRepository articleVisitRepository)
        {
            _userAccountRepository = userAccountRepository;
            _articleVisitRepository = articleVisitRepository;
        }

        [HttpPost]
        public async Task<ActionResult<ApiResponse<ArticleVisit>>> CreateArticleViSit([FromBody] ArticleVisitCreateDTO articleVisitCreateDTO)
        {
            _logger.Info($"User creating article visit of user: ArticleID={articleVisitCreateDTO.ArticleId}");
            UserAccounts? userAccount = null;
            // Email
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail != null)
            {
                userAccount = await _userAccountRepository.GetByEmail(userEmail);
                if (userAccount == null)
                {
                    _logger.Warn($"User account not found for email: {userEmail}");
                    return NotFound(new ApiResponse<ArticleDTO>(404, "Người dùng không tồn tại."));
                }
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


            _logger.Info($"User create article visit of user successfully: ArticleID={articleVisitCreateDTO.ArticleId}");
            return StatusCode(201, new ApiResponse<ArticleVisit>(201, "Ghi nhận truy cập bài viết thành công", addedArticleVisit));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<int>>> GetTotalViewByArticleId(Guid id)
        {

            _logger.Info($"User fetching article view: ArticleID={id}");
            var totalViews = await _articleVisitRepository.CountViewsByArticleIdAsync(id);
            _logger.Info($"User fetching article view successfully: ArticleID={id}");
            return Ok(new ApiResponse<int>(200, $"Tổng số lượt xem cho bài viết", totalViews));
        }
    }
}
