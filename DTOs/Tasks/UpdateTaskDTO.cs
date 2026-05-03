using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.DTOs.Tasks
{
    public class UpdateTaskDTO
    {
        [Required]
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? Priority { get; set; }
        public DateTime? DueDate { get; set; }
    }
}
