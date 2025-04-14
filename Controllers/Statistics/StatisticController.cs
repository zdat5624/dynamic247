using Microsoft.AspNetCore.Mvc;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.Controllers.Statistics
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;

        public StatisticController(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        [HttpGet("user-post-stats")]
        public async Task<ApiResponse<PaginatedResponseDTO<UserPostStatsDTO>>> GetUserPostStats([FromBody] StartEndDateDTO startEndDateDTO)
        {
            var result = await _articleRepository.GetUserPostStats(startEndDateDTO.StartDate, startEndDateDTO.EndDate, startEndDateDTO.PageNumber, startEndDateDTO.PageSize);

            return new ApiResponse<PaginatedResponseDTO<UserPostStatsDTO>>(200, "Lấy dữ liệu thống kê thành công", result);
        }

        [HttpGet("article-view-stats")]
        public async Task<ApiResponse<PaginatedResponseDTO<UserArticleViewStatsDTO>>> GetArticleViewStats([FromBody] ArticleViewStatsRequestDTO articleViewStatsRequestDTO)
        {
            var result = await _articleRepository.GetUserArticleViewStats(articleViewStatsRequestDTO);
            return new ApiResponse<PaginatedResponseDTO<UserArticleViewStatsDTO>>(200, "Lấy thống kê lượt xem bài báo thành công", result);
        }

        [HttpGet("article-comment-stats")]
        public async Task<ApiResponse<PaginatedResponseDTO<ArticleCommentStatsDTO>>> GetArticleCommentStats([FromBody] ArticleCommentStatsRequestDTO articleCommentStatsRequest)
        {
            var result = await _articleRepository.GetUserArticleCommentStats(articleCommentStatsRequest);
            return new ApiResponse<PaginatedResponseDTO<ArticleCommentStatsDTO>>(200, "Lấy thống kê bình luận bài báo thành công", result);
        }


    }
}
