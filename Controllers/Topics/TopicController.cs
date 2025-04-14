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
    public class TopicController : ControllerBase
    {
        private readonly ITopicRepository _topicRepository;

        public TopicController(ITopicRepository topicRepository)
        {
            _topicRepository = topicRepository;
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Topic>>> CreateTopic([FromBody] TopicCreateDTO topicDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new ApiResponse<string>(400, "Dữ liệu không hợp lệ", ModelState.ToString()));

            var topic = new Topic
            {
                Id = Guid.NewGuid(),
                Name = topicDto.Name
            };

            var createdTopic = await _topicRepository.AddTopicAsync(topic);
            var response = new ApiResponse<Topic>(201, "Tạo chủ đề thành công", createdTopic);

            return StatusCode(201, response);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<Topic>>> GetTopicById(Guid id)
        {
            var topic = await _topicRepository.GetTopicByIdAsync(id);
            if (topic == null)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy chủ đề"));

            return Ok(new ApiResponse<Topic>(200, "Lấy chủ đề thành công", topic));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<Topic>>> UpdateTopic(Guid id, [FromBody] Topic updatedTopic)
        {
            if (id != updatedTopic.Id)
                return BadRequest(new ApiResponse<string>(400, "ID không khớp"));

            var result = await _topicRepository.UpdateTopicAsync(updatedTopic);
            if (result == null)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy chủ đề để cập nhật"));

            return Ok(new ApiResponse<Topic>(200, "Cập nhật chủ đề thành công", result));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteTopic(Guid id)
        {
            var isDeleted = await _topicRepository.DeleteTopicAsync(id);
            if (!isDeleted)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy chủ đề để xoá", null));

            return Ok(new ApiResponse<string>(200, "Xóa chủ đề thành công", null));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDTO<Topic>>>> GetPaginatedTopics(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? searchName = null,
            [FromQuery] bool sortByNameAsc = true)
        {
            var paginatedResult = await _topicRepository.GetPaginatedTopicsAsync(
                pageNumber,
                pageSize,
                searchName,
                sortByNameAsc);

            return Ok(new ApiResponse<PaginatedResponseDTO<Topic>>(200, "Lấy danh sách chủ đề thành công", paginatedResult));
        }
    }
}