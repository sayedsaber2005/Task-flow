namespace ProjectManagement.DTOs.Tasks
{
    public class TaskQueryDTO
    {
        public string? Status { get; set; }
        public string? Priority { get; set; }

        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
