namespace ProjectManagement.DTOs.Notifications
{
    public class NotificationDTO
    {
        public int Id { get; set; }
        public string Message { get; set; }
        public string Type { get; set; }
        public DateTime? CreatedAt { get; set; }
    }
}
