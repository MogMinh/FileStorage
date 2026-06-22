using System;

namespace FileStorage.Domain.Entities
{
    public class UserFile
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string BucketName { get; set; } = string.Empty;
        public string ObjectName { get; set; } = string.Empty;     // Đường dẫn trong MinIO
        public string FileName { get; set; } = string.Empty;
        public long SizeBytes { get; set; }
        public string? ContentType { get; set; }
        public DateTime UploadDate { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; } = false;
    }
}