namespace ProjectManagement.DTOs.TaskHistory
{
    public class CreateTaskHistoryDTO
    {
        public int TaskId { get; set; }

        public string FieldChanged { get; set; }

        public string OldValue { get; set; }

        public string NewValue { get; set; }

        public string? ChangedBy { get; set; }
    }
}
