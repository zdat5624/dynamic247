using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NewsPage.Enums;
using NewsPage.helpers;
using NewsPage.Models.entities;
using NewsPage.Models.RequestDTO;
using NewsPage.Models.ResponseDTO;
using NewsPage.repositories.interfaces;
using System.Security.Claims;

namespace NewsPage.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ArticleController : ControllerBase
    {
        private readonly IArticleRepository _articleRepository;
        private readonly IUserDetailRepository _userDetailRepository;
        private readonly IUserAccountRepository _userAccountRepository;

        private readonly ICategoryRepository _categoryRepository;
        private readonly IFavoriteTopicRepository _favoriteTopicRepository;
        private readonly ITopicRepository _topicRepository;


        private readonly MailHelper _mailHelper;


        public ArticleController(IArticleRepository articleRepository, IUserDetailRepository userDetailRepository, IUserAccountRepository userAccountRepository,
        MailHelper mailHelper, ICategoryRepository categoryRepository, IFavoriteTopicRepository favoriteTopicRepository, ITopicRepository topicRepository)
        {
            _articleRepository = articleRepository;
            _userDetailRepository = userDetailRepository;
            _userAccountRepository = userAccountRepository;
            _mailHelper = mailHelper;
            _categoryRepository = categoryRepository;
            _favoriteTopicRepository = favoriteTopicRepository;
            _topicRepository = topicRepository;
        }

        [HttpPost("admin")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ArticleDTO>>> AdminCreateArticle([FromBody] ArticleAdminCreateDTO articleCreateDTO)
        {
            // Mapping DTO
            var newArticle = new Article
            {
                Title = articleCreateDTO.Title,
                Thumbnail = articleCreateDTO.Thumbnail,
                Content = articleCreateDTO.Content,
                Status = articleCreateDTO.Status,
                IsShowAuthor = articleCreateDTO.IsShowAuthor,
                CreateAt = DateTime.UtcNow,
                UserAccountId = articleCreateDTO.UserAccountId,
                CategoryId = articleCreateDTO.CategoryId
            };

            if (newArticle.Status == ArticleStatus.PUBLISHED)
            {
                newArticle.PublishedAt = DateTime.UtcNow;
            }
            else
            {
                newArticle.PublishedAt = null;
            }

            // Create  article
            var createdArticle = await _articleRepository.CreateAsync(newArticle);

            // Fetch UserDetails
            var userDetails = await _userDetailRepository.GetDetailByAccountID(createdArticle.UserAccountId);
            var articleDTO = MapToArticleDTO(createdArticle, userDetails);

            return StatusCode(201, new ApiResponse<ArticleDTO>(201, "Bài viết đã được tạo thành công.", articleDTO));
        }



        [HttpPost("editor")]
        [Authorize(Roles = "Editor")]
        public async Task<ActionResult<ApiResponse<ArticleDTO>>> EditorCreateArticle([FromBody] ArticleEditorCreateDTO articleEditorCreateDTO)
        {
            // Email
            var editorEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (editorEmail == null) return Unauthorized(new ApiResponse<ArticleDTO>(401, "Email không tồn tại trong token."));


            // Fetch UserAccount
            var editorAccount = await _userAccountRepository.GetByEmail(editorEmail);
            if (editorAccount == null) return NotFound(new ApiResponse<ArticleDTO>(404, "Người dùng không tồn tại."));

            // DRAFT hoặc PENDING
            if (articleEditorCreateDTO.Status != ArticleStatus.DRAFT && articleEditorCreateDTO.Status != ArticleStatus.PENDING)
            {
                return BadRequest(new ApiResponse<ArticleDTO>(400, "Editor chỉ được tạo bài viết với trạng thái DRAFT hoặc PENDING."));
            }


            var newArticle = new Article
            {
                Title = articleEditorCreateDTO.Title,
                Thumbnail = articleEditorCreateDTO.Thumbnail,
                Content = articleEditorCreateDTO.Content,
                Status = articleEditorCreateDTO.Status,
                PublishedAt = null,
                IsShowAuthor = articleEditorCreateDTO.IsShowAuthor,
                CreateAt = DateTime.UtcNow,
                UserAccountId = editorAccount.Id, //  UserAccountId của editor
                CategoryId = articleEditorCreateDTO.CategoryId
            };

            var createdArticle = await _articleRepository.CreateAsync(newArticle);

            var userDetails = await _userDetailRepository.GetDetailByAccountID(createdArticle.UserAccountId);
            var articleDTO = MapToArticleDTO(createdArticle, userDetails);

            return StatusCode(201, new ApiResponse<ArticleDTO>(201, "Bài viết đã được tạo thành công.", articleDTO));
        }


        [HttpPut("editor/{id}")]
        [Authorize(Roles = "Editor")]
        public async Task<ActionResult<ApiResponse<ArticleDTO>>> UpdateArticleByEditor(Guid id, [FromBody] ArticleEditorUpdateDTO updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest(new ApiResponse<ArticleDTO>(400, "ID không khớp."));

            var editorEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (editorEmail == null)
                return Unauthorized(new ApiResponse<ArticleDTO>(401, "Email không tồn tại trong token."));

            var editorAccount = await _userAccountRepository.GetByEmail(editorEmail);
            if (editorAccount == null)
                return NotFound(new ApiResponse<ArticleDTO>(404, "Người dùng không tồn tại."));

            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null)
                return NotFound(new ApiResponse<ArticleDTO>(404, "Bài viết không tồn tại."));

            if (article.UserAccountId != editorAccount.Id ||
                (article.Status != ArticleStatus.PENDING && article.Status != ArticleStatus.DRAFT && article.Status != ArticleStatus.REJECTED))
                return StatusCode(403, new ApiResponse<ArticleDTO>(403, "Bạn chỉ có thể chỉnh sửa bài viết của mình với trạng thái DRAFT, PENDING hoặc REJECTED."));

            article.Title = updateDto.Title;
            article.Thumbnail = updateDto.Thumbnail;
            article.Content = updateDto.Content;
            article.IsShowAuthor = updateDto.IsShowAuthor;
            article.CategoryId = updateDto.CategoryId;

            var updatedArticle = await _articleRepository.UpdateAsync(article);
            if (updatedArticle == null)
                return NotFound(new ApiResponse<ArticleDTO>(404, "Không thể tìm thấy bài viết sau khi cập nhật."));

            var userDetails = await _userDetailRepository.GetDetailByAccountID(updatedArticle.UserAccountId);
            var updatedArticleDTO = MapToArticleDTO(updatedArticle, userDetails);

            return Ok(new ApiResponse<ArticleDTO>(200, "Bài viết đã được cập nhật thành công.", updatedArticleDTO));
        }


        [HttpPut("admin/{id}")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<ArticleDTO>>> UpdateArticleByAdmin(Guid id, [FromBody] ArticleAdminUpdateDTO updateDto)
        {
            if (id != updateDto.Id)
                return BadRequest(new ApiResponse<ArticleDTO>(400, "ID không khớp."));

            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null)
                return NotFound(new ApiResponse<ArticleDTO>(404, "Bài viết không tồn tại."));

            article.Title = updateDto.Title;
            article.Thumbnail = updateDto.Thumbnail;
            article.Content = updateDto.Content;
            article.UserAccountId = updateDto.UserAccountId ?? article.UserAccountId;
            article.CategoryId = updateDto.CategoryId;

            if (article.PublishedAt != null)
                article.UpdateAt = DateTime.UtcNow;

            var updatedArticle = await _articleRepository.UpdateAsync(article);
            if (updatedArticle == null)
                return NotFound(new ApiResponse<ArticleDTO>(404, "Không thể tìm thấy bài viết sau khi cập nhật."));

            var userDetails = await _userDetailRepository.GetDetailByAccountID(updatedArticle.UserAccountId);
            var updatedArticleDTO = MapToArticleDTO(updatedArticle, userDetails);

            return Ok(new ApiResponse<ArticleDTO>(200, "Bài viết đã được cập nhật thành công.", updatedArticleDTO));
        }




        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResponse<ArticleDTO>>> GetArticle(Guid id)
        {
            var article = await _articleRepository.GetArticleWithCategoryAsync(id);
            if (article == null)
                return NotFound(new ApiResponse<ArticleDTO>(404, "Bài viết không tồn tại."));

            var userDetails = await _userDetailRepository.GetDetailByAccountID(article.UserAccountId);
            var articleDTO = MapToArticleDTO(article, userDetails);

            return Ok(new ApiResponse<ArticleDTO>(200, "Lấy thông tin bài viết thành công.", articleDTO));
        }

        [HttpPut("update-status/{id}")]
        [Authorize(Roles = "Editor,Admin")]
        public async Task<ActionResult<ApiResponse<string>>> UpdateArticleStatus(Guid id, [FromBody] ArticleStatusUpdateDTO statusUpdateDTO)
        {
            var userEmail = User.FindFirst(ClaimTypes.Name)?.Value;
            if (userEmail == null)
                return Unauthorized(new ApiResponse<string>(401, "Email không tồn tại trong token."));

            var userAccount = await _userAccountRepository.GetByEmail(userEmail);
            if (userAccount == null)
                return NotFound(new ApiResponse<string>(404, "Người dùng không tồn tại."));

            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null)
                return NotFound(new ApiResponse<string>(404, "Bài viết không tồn tại."));

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Editor")
            {
                if (article.UserAccountId != userAccount.Id)
                    return StatusCode(403, new ApiResponse<string>(403, "Bạn chỉ có thể cập nhật trạng thái bài viết của chính mình."));

                if (article.Status != ArticleStatus.DRAFT && article.Status != ArticleStatus.PENDING && article.Status != ArticleStatus.REJECTED)
                    return BadRequest(new ApiResponse<string>(400, "Editor chỉ có thể cập nhật trạng thái bài viết với trạng thái hiện tại là DRAFT, PENDING hoặc REJECTED."));

                if (statusUpdateDTO.Status != ArticleStatus.DRAFT && statusUpdateDTO.Status != ArticleStatus.PENDING)
                    return BadRequest(new ApiResponse<string>(400, "Editor chỉ được phép cập nhật trạng thái thành DRAFT hoặc PENDING."));
            }

            article.Status = statusUpdateDTO.Status;
            if (statusUpdateDTO.Status == ArticleStatus.PUBLISHED && article.PublishedAt == null){

                //send notify to interested reader
                try
                {
                    // Get the category to determine the topic
                    var category = await _categoryRepository.GetByIdAsync(article.CategoryId);
                    if (category == null || category.TopicId == null) throw new Exception("No category was found");

                    // Get users who have marked this topic as favorite
                    var topicId = category.TopicId;
                    var interestedUsers = await _favoriteTopicRepository.GetUsersByTopicIdAsync(topicId);
                    
                    if (interestedUsers == null || !interestedUsers.Any()) throw new Exception("No interested user was found");

                    // Get all user accounts for the interested users
                    var userEmails = new List<string>();
                    foreach (Guid userId in interestedUsers)
                    {
                        var user = await _userAccountRepository.GetById(userId);
                        if (user != null && !string.IsNullOrEmpty(user.Email))
                        {
                            userEmails.Add(user.Email);
                        }
                    }

                    if (!userEmails.Any()) throw new Exception("No interested user email was found");

                    // Get topic name
                    var topic = await _topicRepository.GetTopicByIdAsync(topicId);
                    var topicName = topic?.Name ?? "chủ đề này";

                    // Send email to all interested users
                    string subject = $"Bài viết mới về {topicName}";
                    string body = $@"
                        <html>
                        <body>
                            <h2>Một bài viết mới vừa được tạo về {topicName} mà bạn quan tâm!</h2>
                            <p><strong>Tiêu đề:</strong> {article.Title}</p>
                            <p>Hãy truy cập vào trang web của chúng tôi để đọc bài viết này khi nó được xuất bản.</p>
                            <p>Cảm ơn bạn đã theo dõi!</p>
                        </body>
                        </html>";

                    await _mailHelper.SendEmailToMultipleRecipientsAsync(userEmails, subject, body);
                }
                catch (Exception)
                {
                    // Log the exception but don't stop the article creation process
                    // Consider adding proper logging here
            }
                article.PublishedAt = DateTime.UtcNow;
            }

            var updatedArticle = await _articleRepository.UpdateAsync(article);
            if (updatedArticle == null)
                return NotFound(new ApiResponse<string>(404, "Không thể tìm thấy bài viết sau khi cập nhật."));

            return Ok(new ApiResponse<string>(200, "Trạng thái bài viết đã được cập nhật thành công."));
        }

        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,Editor")]
        public async Task<ActionResult<ApiResponse<string>>> DeleteArticle(Guid id)
        {
            var article = await _articleRepository.GetByIdAsync(id);
            if (article == null)
                return NotFound(new ApiResponse<string>(404, "Bài viết không tồn tại."));

            var userRole = User.FindFirst(ClaimTypes.Role)?.Value;
            if (userRole == "Editor")
            {
                var editorEmail = User.FindFirst(ClaimTypes.Name)?.Value;
                if (editorEmail == null)
                    return Unauthorized(new ApiResponse<string>(401, "Email không tồn tại trong token."));

                var editorAccount = await _userAccountRepository.GetByEmail(editorEmail);
                if (editorAccount == null)
                    return NotFound(new ApiResponse<string>(404, "Người dùng không tồn tại."));

                if (article.UserAccountId != editorAccount.Id ||
                    (article.Status != ArticleStatus.DRAFT && article.Status != ArticleStatus.PENDING && article.Status != ArticleStatus.REJECTED))
                    return StatusCode(403, new ApiResponse<string>(403, "Bạn chỉ có thể xóa bài viết của mình với trạng thái DRAFT, PENDING hoặc REJECTED."));
            }

            await _articleRepository.DeleteAsync(article);
            return Ok(new ApiResponse<string>(200, "Bài viết đã được xóa thành công."));
        }

        [HttpGet("filter")]
        public async Task<ActionResult<ApiResponse<PaginatedResponseDTO<ArticleDTO>>>> GetFilteredArticles(
            [FromQuery] ArticleFilterRequestDTO filterRequest)
        {
            var paginatedArticles = await _articleRepository.GetPaginatedArticlesAsync(
                filterRequest.PageNumber,
                filterRequest.PageSize,
                filterRequest.SearchTerm,
                filterRequest.CategoryId,
                filterRequest.UserAccountId,
                filterRequest.PublishedAt,
                filterRequest.Status,
                filterRequest.TopicId,
                filterRequest.SortBy,
                filterRequest.SortOrder
            );

            var articleDTOs = new List<ArticleDTO>();
            foreach (var article in paginatedArticles.Items)
            {
                var userDetails = await _userDetailRepository.GetDetailByAccountID(article.UserAccountId);
                articleDTOs.Add(MapToArticleDTO(article, userDetails));
            }

            var response = new PaginatedResponseDTO<ArticleDTO>(
                articleDTOs,
                paginatedArticles.TotalCount,
                paginatedArticles.PageNumber,
                paginatedArticles.PageSize);

            return Ok(new ApiResponse<PaginatedResponseDTO<ArticleDTO>>(200, "Lấy danh sách bài viết thành công.", response));
        }

        private ArticleDTO MapToArticleDTO(Article article, UserDetails userDetails)
        {
            return new ArticleDTO
            {
                Id = article.Id,
                Title = article.Title ?? string.Empty,
                Thumbnail = article.Thumbnail ?? string.Empty,
                Content = article.Content ?? string.Empty,
                Status = article.Status,
                PublishedAt = article.PublishedAt,
                UpdateAt = article.UpdateAt,
                CreateAt = article.CreateAt,
                Category = article.Category == null ? null : new CategoryDTO
                {
                    Id = article.Category.Id,
                    Name = article.Category.Name,
                    Topic = article.Category.Topic == null ? null : new Topic
                    {
                        Id = article.Category.Topic.Id,
                        Name = article.Category.Topic.Name
                    }
                },
                UserDetails = userDetails,
                UserAccountEmail = article.UserAccounts?.Email
            };
        }

    }
}
