namespace ProjectManagement.DTOs.TaskHistory
{
    public class TaskHistoryDTO
    {
        public int Id { get; set; }
        public int? TaskId { get; set; }
        public string? ChangedBy { get; set; }
        public string? FieldChanged { get; set; }
        public string? OldValue { get; set; }
        public string? NewValue { get; set; }
        public DateTime? ChangedAt { get; set; }
    }
}
