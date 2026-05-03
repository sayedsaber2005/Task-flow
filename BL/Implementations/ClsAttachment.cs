using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Attachments;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;

namespace ProjectManagement.BL.Implementations
{
    public class ClsAttachment : IAttachment
    {
        private readonly IAttachmentRepository _repo;
        private readonly IWebHostEnvironment _env;

        public ClsAttachment(IAttachmentRepository repo, IWebHostEnvironment env)
        {
            _repo = repo;
            _env = env;
        }

        public ApiResponse GetByTaskId(int taskId)
        {
            var attachments = _repo.GetByTaskId(taskId);

            var result = attachments.Select(a => new AttachmentDTO
            {
                Id = a.Id,
                FileUrl = a.FileUrl,
                CreatedAt = a.CreatedAt
            });

            return new ApiResponse
            {
                Data = result,
                StatusCode = "200"
            };
        }

        public async Task<ApiResponse> Upload(int taskId, string userId, IFormFile file)
        {
            var result = new ApiResponse();

            if (file == null || file.Length == 0)
            {
                result.Errors.Add(new { Message = "File is required" });
                result.StatusCode = "400";
                return result;
            }

            // create folder
            var uploadsFolder = Path.Combine(_env.WebRootPath, "uploads");

            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            // unique file name
            var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
            var filePath = Path.Combine(uploadsFolder, fileName);

            // save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // save in DB
            var attachment = new TbAttachment
            {
                TaskId = taskId,
                UploadedBy = userId,
                FileUrl = "/uploads/" + fileName,
                CreatedAt = DateTime.UtcNow
            };

            _repo.Add(attachment);
            _repo.Save();

            result.Data = new AttachmentDTO
            {
                Id = attachment.Id,
                FileUrl = attachment.FileUrl,
                CreatedAt = attachment.CreatedAt
            };
            result.StatusCode = "201";

            return result;
        }

        public ApiResponse Delete(int attachmentId, string userId)
        {
            var result = new ApiResponse();

            var attachment = _repo.GetById(attachmentId);

            if (attachment == null)
            {
                result.Errors.Add(new { Message = "Not found" });
                result.StatusCode = "404";
                return result;
            }

            // authorization
            if (attachment.UploadedBy != userId)
            {
                result.Errors.Add(new { Message = "Not allowed" });
                result.StatusCode = "403";
                return result;
            }

            // delete file from server
            var filePath = Path.Combine(_env.WebRootPath, attachment.FileUrl.TrimStart('/'));

            if (File.Exists(filePath))
                File.Delete(filePath);

            _repo.Delete(attachment);
            _repo.Save();

            result.StatusCode = "200";
            return result;
        }
    }
}
