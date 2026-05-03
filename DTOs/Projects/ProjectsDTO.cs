namespace ProjectManagement.DTOs.Projects
{
    public class ProjectsDTO
    {
        public int Id { get; set; }

        public string Name { get; set; }

        public string Description { get; set; }

        public string CreatedBy { get; set; }

        public DateTime? CreatedAt { get; set; }
        public int TotalTasks { get; set; }
        public int TotalMembers { get; set; }
    }
}
