using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPage.Models.entities;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.Controllers.Topics
{
    [Route("/api/v1/[controller]")]
    [ApiController]
    public class FavoriteTopicController : ControllerBase
    {
        private readonly IFavoriteTopicRepository _favoriteTopicRepository;

        public FavoriteTopicController(IFavoriteTopicRepository favoriteTopicRepository)
        {
            _favoriteTopicRepository = favoriteTopicRepository;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<ApiResponse<FavoriteTopics>>> AddFavoriteTopic([FromBody] FavoriteTopicCreateDTO favoriteTopicDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>(400, "Dữ liệu không hợp lệ", ModelState.ToString()));

            var favoriteTopic = new FavoriteTopics
            {
                Id = Guid.NewGuid(),
                UserId = favoriteTopicDto.UserId,
                TopicId = favoriteTopicDto.TopicId
            };

            var createdFavoriteTopic = await _favoriteTopicRepository.AddFavoriteTopicAsync(favoriteTopic);
            var response = new ApiResponse<FavoriteTopics>(201, "Thêm chủ đề yêu thích thành công", createdFavoriteTopic);
            return StatusCode(201, response);
        }

       
        [HttpDelete("user/{userId}/topic/{topicId}")]
        [Authorize]
        public async Task<ActionResult<ApiResponse<string>>> DeleteFavoriteTopicByUserAndTopic(Guid userId, Guid topicId)
        {
            var isDeleted = await _favoriteTopicRepository.DeleteFavoriteTopicByUserAndTopicAsync(userId, topicId);
            if (!isDeleted)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy chủ đề yêu thích để xoá", null));

            return Ok(new ApiResponse<string>(200, "Xóa chủ đề yêu thích thành công", null));
        }

        [HttpGet]
        [Authorize]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDTO<FavoriteTopics>>>> GetFavoriteTopics(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] Guid? userId = null,
            [FromQuery] Guid? topicId = null)
        {
            var paginatedResult = await _favoriteTopicRepository.GetPaginatedFavoriteTopicsAsync(
                pageNumber,
                pageSize,
                userId,
                topicId);

            string message = userId.HasValue 
                ? "Lấy danh sách chủ đề yêu thích của người dùng thành công" 
                : "Lấy danh sách chủ đề yêu thích thành công";

            return Ok(new ApiResponse<PaginatedResponseDTO<FavoriteTopics>>(200, message, paginatedResult));
        }
    }
}