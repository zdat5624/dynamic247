using Microsoft.AspNetCore.Authorization;
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
    [Authorize]
    public class ArticleStorageController : ControllerBase
    {
        private readonly IArticleStorageRepository _articleStorageRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private static readonly Logger _logger = LogManager.GetCurrentClassLogger();

        public ArticleStorageController(IArticleStorageRepository articleStorageRepository, IUserAccountRepository userAccountRepository)
        {
            _articleStorageRepository = articleStorageRepository;
            _userAccountRepository = userAccountRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<ArticleStorage>>> CreateArticleStorage([FromBody] ArticleStorageCreateDTO articleStorageCreateDTO)
        {
            _logger.Info($"User saving article: {articleStorageCreateDTO.ArticleId}");
            UserAccounts? userAccount = null;
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail != null)
            {
                userAccount = await _userAccountRepository.GetByEmail(userEmail);
                if (userAccount == null)
                {
                    _logger.Warn("No email found in token.");
                    return NotFound(new ApiResponse<ArticleStorage>(404, "Người dùng không tồn tại."));
                }
            }
            else
            {
                _logger.Warn($"User account not found for email: {userEmail}");
                return Unauthorized(new ApiResponse<ArticleStorage>(401, "Không tìm thấy  người dùng."));
            }

            var articleStorage = new ArticleStorage
            {
                ArticleId = articleStorageCreateDTO.ArticleId,
                UserAccountId = userAccount.Id,
                CreateAt = DateTime.UtcNow
            };

            var addedArticleStorage = await _articleStorageRepository.CreateAsync(articleStorage);
            if (addedArticleStorage == null)
            {
                _logger.Warn($"user save duplicate article: {articleStorage.Id}");
                return BadRequest(new ApiResponse<ArticleStorage>(400, "Bài viết đã được lưu bởi người dùng."));
            }


            _logger.Info($"User save article successfully: {articleStorageCreateDTO.ArticleId}");
            return StatusCode(201, new ApiResponse<ArticleStorage>(201, "Lưu bài viết thành công.", addedArticleStorage));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDTO<ArticleStorage>>>> GetArticleStorageOfUser([FromBody] PagingDTO request)
        {
            _logger.Info($"User fetching article storeaged");

            UserAccounts? userAccount = null;
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail != null)
            {
                userAccount = await _userAccountRepository.GetByEmail(userEmail);
                if (userAccount == null)
                {
                    _logger.Warn("No email found in token.");
                    return NotFound(new ApiResponse<PaginatedResponseDTO<ArticleStorage>>(404, "Người dùng không tồn tại."));
                }
            }
            else
            {
                _logger.Warn($"User account not found for email: {userEmail}");
                return Unauthorized(new ApiResponse<PaginatedResponseDTO<ArticleStorage>>(401, "Không tìm thấy thông tin người dùng."));
            }

            var paginatedResult = await _articleStorageRepository.GetArticleStorageOfUser(userAccount.Id, request.pageNumber, request.pageSize);

            _logger.Info($"User fetching article successfully");

            return Ok(new ApiResponse<PaginatedResponseDTO<ArticleStorage>>(200, "Lấy danh sách bài viết đã lưu thành công.", paginatedResult));
        }

        [HttpGet("check")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<bool>>> CheckArticleStored([FromQuery] Guid articleId)
        {
            _logger.Info($"User checking article stored: article ID={articleId}");

            UserAccounts? userAccount = null;
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail != null)
            {
                userAccount = await _userAccountRepository.GetByEmail(userEmail);
                if (userAccount == null)
                {
                    _logger.Warn("No email found in token.");
                    return NotFound(new ApiResponse<bool>(404, "Người dùng không tồn tại."));
                }
            }
            else
            {
                _logger.Warn($"User account not found for email: {userEmail}");
                return Unauthorized(new ApiResponse<bool>(401, "Không tìm thấy thông tin người dùng."));
            }

            var isStored = await _articleStorageRepository.IsArticleStoredByUser(userAccount.Id, articleId);

            _logger.Info($"User check article stored successfully: article ID={articleId}");
            return Ok(new ApiResponse<bool>(200, "Kiểm tra trạng thái lưu bài viết thành công.", isStored));
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<object>>> DeleteArticleStorage(Guid id)
        {
            _logger.Info($"User deleting article stored: article ID={id}");
            UserAccounts? userAccount = null;
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail != null)
            {
                userAccount = await _userAccountRepository.GetByEmail(userEmail);
                if (userAccount == null)
                {
                    _logger.Warn("No email found in token.");
                    return NotFound(new ApiResponse<object>(404, "Người dùng không tồn tại."));
                }
            }
            else
            {
                _logger.Warn($"User account not found for email: {userEmail}");
                return Unauthorized(new ApiResponse<object>(401, "Không tìm thấy thông tin người dùng."));
            }

            // Kiểm tra xem ArticleStorage có thuộc về người dùng hay không
            var articleStorage = await _articleStorageRepository.GetAsync(id, userAccount.Id);
            if (articleStorage == null)
            {
                _logger.Warn($"Not found article to delete: ID={id}");
                return NotFound(new ApiResponse<object>(404, "Không tìm thấy bài viết trong danh sách lưu trữ hoặc bạn không có quyền xóa."));
            }


            await _articleStorageRepository.DeleteAsync(id);
            _logger.Info($"User deleting article successfully: article ID={id}");

            return Ok(new ApiResponse<object>(200, "Xóa bài viết khỏi danh sách lưu trữ thành công."));
        }


    }

}


