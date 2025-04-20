using System.Text.Json;
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
        private readonly IPageVisitorRepository _pageVisitorRepository;
        private readonly IReadingFrequencyRepository _readIReadingFrequencyRepository;


        public StatisticController(IArticleRepository articleRepository, IPageVisitorRepository pageVisitorRepository,
        IReadingFrequencyRepository readIReadingFrequencyRepository)
        {
            _articleRepository = articleRepository;
            _pageVisitorRepository = pageVisitorRepository;
            _readIReadingFrequencyRepository = readIReadingFrequencyRepository;
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
        [HttpGet("page-visit-stats")]
        public async Task<IActionResult> GetPageVisitStats()
        {
            try
            {
                PageVisitResponseDTO p = await _pageVisitorRepository.GetTotalViews();
                var response = new PageVisitResponseDTO
                {
                    DailyStats = p.DailyStats,
                    MonthlyStats = p.MonthlyStats,
                    YearlyStats = p.YearlyStats
                };
                return Ok(response);
            }
            catch (Exception e)
            {
                // Log the exception (e.g., _logger.LogError(e, "Error retrieving page visit stats");)
                return StatusCode(500, new { Error = "An error occurred while retrieving page visit stats" });
            }
        }
        [HttpGet("reading-freq-user-stats/{id}")]
        public async Task<IActionResult> GetReadingFreqOfUser(Guid id)
        {
            try
            {
               var freqs = await _readIReadingFrequencyRepository.GetUserReadingFrequencyAsync(id);
                return Ok(freqs);
            }
            catch (Exception e)
            {
                // Log the exception (e.g., _logger.LogError(e, "Error retrieving page visit stats");)
                return StatusCode(500, new { Error = "An error occurred while retrieving page visit stats" });
            }
        }
        [HttpGet("reading-freq-all-user-stats")]
        public async Task<IActionResult> GetReadingFreqOfAllUser([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                if (pageNumber < 1)
                    pageNumber = 1;
                    
                if (pageSize < 1 || pageSize > 100)
                    pageSize = 10;
                    
                var (items, totalCount) = await _readIReadingFrequencyRepository.GetAllUserReadingFrequencyPaginatedAsync(pageNumber, pageSize);
                
                // Create pagination metadata
                var paginationMetadata = new
                {
                    TotalCount = totalCount,
                    PageSize = pageSize,
                    CurrentPage = pageNumber,
                    TotalPages = (int)Math.Ceiling(totalCount / (double)pageSize)
                };
                
                // Add pagination metadata to response headers
                Response.Headers.Add("X-Pagination", JsonSerializer.Serialize(paginationMetadata));
                
                return Ok(new
                {
                    Items = items,
                    Pagination = paginationMetadata
                });
            }
            catch (Exception e)
            {
                // Log the exception (e.g., _logger.LogError(e, "Error retrieving reading frequency stats");)
                return StatusCode(500, new { Error = "An error occurred while retrieving reading frequency stats" });
            }
        }


    }
}
