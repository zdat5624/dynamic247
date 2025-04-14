using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPage.Models.entities;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;

namespace NewsPage.Controllers.Categories
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITopicRepository _topicRepository;

        public CategoryController(ICategoryRepository categoryRepository, ITopicRepository topicRepository)
        {
            _categoryRepository = categoryRepository;
            _topicRepository = topicRepository;
        }

        [HttpGet("topic/{topicId}")]
        public async Task<ActionResult<ApiResponse<IEnumerable<CategoryDTO>>>> GetCategoriesByTopicId(Guid topicId)
        {
            var categories = await _categoryRepository.GetCategoriesByTopicIdAsync(topicId);
            if (categories == null || !categories.Any())
                return NotFound(new ApiResponse<IEnumerable<CategoryDTO>>(404, "Không tìm thấy danh mục nào", null));

            var categoryDTOs = categories.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name, Topic = c.Topic }).ToList();
            return Ok(new ApiResponse<IEnumerable<CategoryDTO>>(200, "Lấy danh mục thành công", categoryDTOs));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> GetCategory(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse<CategoryDTO>(404, "Không tìm thấy danh mục", null));

            var categoryDTO = new CategoryDTO { Id = category.Id, Name = category.Name, Topic = category.Topic };
            return Ok(new ApiResponse<CategoryDTO>(200, "Lấy danh mục thành công", categoryDTO));
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> CreateCategory([FromBody] CategoryCreateDTO categoryCreateDTO)
        {
            var topic = await _topicRepository.GetTopicByIdAsync(categoryCreateDTO.TopicId);
            if (topic == null)
                return NotFound(new ApiResponse<CategoryDTO>(404, "Không tìm thấy topic", null));

            var category = new Category { Id = Guid.NewGuid(), Name = categoryCreateDTO.Name, TopicId = categoryCreateDTO.TopicId };
            var addedCategory = await _categoryRepository.AddAsync(category);

            var categoryDTO = new CategoryDTO { Id = addedCategory.Id, Name = addedCategory.Name, Topic = addedCategory.Topic };
            return StatusCode(201, new ApiResponse<CategoryDTO>(201, "Tạo danh mục thành công", categoryDTO));
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<CategoryDTO>>> UpdateCategory(Guid id, [FromBody] CategoryUpdateDTO categoryDto)
        {
            if (id != categoryDto.Id)
                return BadRequest(new ApiResponse<CategoryDTO>(400, "ID không khớp", null));

            var existingCategory = await _categoryRepository.GetByIdAsync(id);
            if (existingCategory == null)
                return NotFound(new ApiResponse<CategoryDTO>(404, "Danh mục không tồn tại", null));

            existingCategory.Name = categoryDto.Name;
            existingCategory.TopicId = categoryDto.TopicId;
            var updatedCategory = await _categoryRepository.UpdateAsync(existingCategory);

            if (updatedCategory == null)
                return BadRequest(new ApiResponse<CategoryDTO>(400, "Cập nhật thất bại", null));

            var updatedCategoryDTO = new CategoryDTO { Id = updatedCategory.Id, Name = updatedCategory.Name, Topic = updatedCategory.Topic };
            return Ok(new ApiResponse<CategoryDTO>(200, "Cập nhật danh mục thành công", updatedCategoryDTO));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteCategory(Guid id)
        {
            var category = await _categoryRepository.GetByIdAsync(id);
            if (category == null)
                return NotFound(new ApiResponse<string>(404, "Không tìm thấy danh mục", null));

            await _categoryRepository.DeleteAsync(id);
            return Ok(new ApiResponse<string>(200, "Xóa danh mục thành công", null));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDTO<CategoryDTO>>>> GetPaginatedCategories([FromQuery] CategoryFilterRequestDTO filter)
        {
            var paginatedResult = await _categoryRepository.GetPaginatedCategoriesAsync(
                filter.PageNumber, filter.PageSize, filter.SearchName, filter.TopicId, filter.SortByNameAsc);

            var categoryDTOs = paginatedResult.Items.Select(c => new CategoryDTO { Id = c.Id, Name = c.Name, Topic = c.Topic }).ToList();
            var response = new PaginatedResponseDTO<CategoryDTO>(categoryDTOs, paginatedResult.TotalCount, paginatedResult.PageNumber, paginatedResult.PageSize);

            return Ok(new ApiResponse<PaginatedResponseDTO<CategoryDTO>>(200, "Lấy danh sách danh mục thành công", response));
        }
    }
}
