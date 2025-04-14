using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPage.Models.entities;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;
using System.Security.Claims;

namespace NewsPage.Controllers.Comments
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly ICommentRepository _commentRepository;
        private readonly IUserAccountRepository _userAccountRepository;
        private readonly IArticleRepository _articleRepository;

        public CommentController(ICommentRepository commentRepository, IUserAccountRepository userAccountRepository, IArticleRepository articleRepository)
        {
            _commentRepository = commentRepository;
            _userAccountRepository = userAccountRepository;
            _articleRepository = articleRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> CreateComment([FromBody] CommentCreateDTO request)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail == null)
                return Unauthorized(new ApiResponse<CommentResponseDTO>(401, "Email không tồn tại trong token."));

            var userAccount = await _userAccountRepository.GetByEmail(userEmail);
            if (userAccount == null)
                return NotFound(new ApiResponse<CommentResponseDTO>(404, "Người dùng không tồn tại."));

            var article = await _articleRepository.GetByIdAsync(request.ArticleId);
            if (article == null)
                return NotFound(new ApiResponse<CommentResponseDTO>(404, "Bài viết không tồn tại."));

            var comment = new Comment
            {
                Id = Guid.NewGuid(),
                Content = request.Content,
                ArticleId = request.ArticleId,
                UserAccountId = userAccount.Id,
                CreatedAt = DateTime.UtcNow,
                IsHiden = false
            };

            var createdComment = await _commentRepository.CreateAsync(comment);
            var response = MapToDTO(createdComment);

            return Ok(new ApiResponse<CommentResponseDTO>(200, "Bình luận đã được tạo thành công.", response));
        }

        [Authorize]
        [HttpDelete("{commentId}")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteComment(Guid commentId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail == null)
                return Unauthorized(new ApiResponse<string>(401, "Email không tồn tại trong token."));

            var userAccount = await _userAccountRepository.GetByEmail(userEmail);
            if (userAccount == null)
                return NotFound(new ApiResponse<string>(404, "Người dùng không tồn tại."));

            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return NotFound(new ApiResponse<string>(404, "Bình luận không tồn tại."));

            var article = await _articleRepository.GetByIdAsync(comment.ArticleId);
            if (article == null)
                return NotFound(new ApiResponse<string>(404, "Bài viết chứa bình luận này không tồn tại."));

            if (comment.UserAccountId != userAccount.Id && userAccount.Role != "Admin" && article.UserAccountId != userAccount.Id)
                return StatusCode(403, new ApiResponse<string>(403, "Bạn không có quyền xóa bình luận này."));

            await _commentRepository.DeleteAsync(comment);

            return Ok(new ApiResponse<string>(200, "Bình luận đã được xóa thành công."));
        }

        [Authorize]
        [HttpPut("toggle-visibility/{commentId}")]
        public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> ToggleCommentVisibility(Guid commentId)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail == null)
                return Unauthorized(new ApiResponse<CommentResponseDTO>(401, "Email không tồn tại trong token."));

            var userAccount = await _userAccountRepository.GetByEmail(userEmail);
            if (userAccount == null)
                return NotFound(new ApiResponse<CommentResponseDTO>(404, "Người dùng không tồn tại."));

            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return NotFound(new ApiResponse<CommentResponseDTO>(404, "Bình luận không tồn tại."));

            var article = await _articleRepository.GetByIdAsync(comment.ArticleId);
            if (article == null)
                return NotFound(new ApiResponse<CommentResponseDTO>(404, "Bài viết chứa bình luận này không tồn tại."));

            if (userAccount.Role != "Admin" && article.UserAccountId != userAccount.Id)
                return StatusCode(403, new ApiResponse<CommentResponseDTO>(403, "Bạn không có quyền thay đổi trạng thái hiển thị của bình luận này."));

            comment.IsHiden = !comment.IsHiden;
            var updatedComment = await _commentRepository.UpdateAsync(comment);

            return Ok(new ApiResponse<CommentResponseDTO>(200, $"Bình luận đã được {(comment.IsHiden ? "ẩn" : "hiển thị")} thành công.", MapToDTO(updatedComment)));
        }

        [Authorize]
        [HttpPut("update-content/{commentId}")]
        public async Task<ActionResult<ApiResponse<CommentResponseDTO>>> UpdateCommentContent(Guid commentId, [FromBody] CommentContentUpdateDTO updateDTO)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail == null)
                return Unauthorized(new ApiResponse<CommentResponseDTO>(401, "Email không tồn tại trong token."));

            var userAccount = await _userAccountRepository.GetByEmail(userEmail);
            if (userAccount == null)
                return NotFound(new ApiResponse<CommentResponseDTO>(404, "Người dùng không tồn tại."));

            var comment = await _commentRepository.GetByIdAsync(commentId);
            if (comment == null)
                return NotFound(new ApiResponse<CommentResponseDTO>(404, "Bình luận không tồn tại."));

            comment.Content = updateDTO.Content;
            var updatedComment = await _commentRepository.UpdateAsync(comment);

            return Ok(new ApiResponse<CommentResponseDTO>(200, "Nội dung bình luận đã được cập nhật thành công.", MapToDTO(updatedComment)));
        }

        [HttpGet("article/{articleId}")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDTO<CommentResponseDTO>>>> GetCommentsByArticle(Guid articleId, int pageNumber = 1, int pageSize = 10, bool descending = false)
        {
            var paginatedComments = await _commentRepository.GetCommentsByArticlePaginatedAsync(articleId, pageNumber, pageSize, descending);

            var result = paginatedComments.Items.Select(MapToDTO).ToList();
            var response = new PaginatedResponseDTO<CommentResponseDTO>(
                items: result,
                totalCount: paginatedComments.TotalCount,
                pageNumber: paginatedComments.PageNumber,
                pageSize: paginatedComments.PageSize
            );

            return Ok(new ApiResponse<PaginatedResponseDTO<CommentResponseDTO>>(200, "Lấy danh sách bình luận thành công.", response));
        }

        private CommentResponseDTO MapToDTO(Comment comment)
        {
            return new CommentResponseDTO
            {
                Id = comment.Id,
                Content = comment.Content,
                IsHiden = comment.IsHiden,
                CreatedAt = comment.CreatedAt,
                ArticleId = comment.ArticleId,
                UserAccountId = comment.UserAccountId
            };
        }
    }
}