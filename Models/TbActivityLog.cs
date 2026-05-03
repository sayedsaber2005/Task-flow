namespace ProjectManagement.Models
{
    public class TbActivityLog
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public ApplicationUser User { get; set; }

        public string Action { get; set; }        // created, updated, deleted
        public string EntityType { get; set; }    // Task, Project, User
        public string EntityId { get; set; }

        public string Description { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
