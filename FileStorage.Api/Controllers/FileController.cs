using FileStorage.Application.Interfaces;
using FileStorage.Domain;
using FileStorage.Domain.Constants;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FileStorage.Api.Controllers
{
    [Route("api/files")]
    [ApiController]
    [Authorize]  
    public class FileController : ControllerBase
    {
        private readonly IMinioService _minioService;
        private readonly IUserRepository _userRepository;

        public FileController(IMinioService minioService, IUserRepository userRepository)
        {
            _minioService = minioService;
            _userRepository = userRepository;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null || file.Length > (user.MaxStorageBytes - user.UsedStorageBytes))
            {
                return BadRequest(new { Success = false, ErrorCode = ErrorCodes.QuotaExceeded, Message = "Vượt quá dung lượng cho phép" });
            }

            try
            {
                var objectName = await _minioService.UploadFileAsync(file, userId);

                await _userRepository.UpdateUsedStorageAsync(userId, file.Length);

                return Ok(new { Success = true, ObjectName = objectName, Message = "Upload thành công" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { Success = false, ErrorCode = ErrorCodes.InternalError, Message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var userId = int.Parse(User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)!.Value);
            var files = await _minioService.ListFilesAsync(userId);
            return Ok(new { Success = true, Data = files });
        }
    }
}