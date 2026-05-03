using System.ComponentModel.DataAnnotations;

namespace ProjectManagement.DTOs.Tasks
{
    public class CreateTaskDTO
    {
        [Required]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required]
        public int ProjectId { get; set; }
        [Required]
        public DateTime DueDate { get; set; }
        [Required]
        public string Priority { get; set; }
    }
}
