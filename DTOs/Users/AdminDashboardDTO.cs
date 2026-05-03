namespace ProjectManagement.DTOs.Users
{
    public class AdminDashboardDTO
    {
        public int TotalUsers { get; set; }
        public int TotalProjects { get; set; }
        public int TotalTasks { get; set; }

        public int TotalProjectManagers { get; set; }
        public int PendingProjectManagers { get; set; }
        public int ApprovedProjectManagers { get; set; }

        public int DoneTasks { get; set; }
        public int PendingTasks { get; set; }
        public int OverDueTasks { get; set; }

        public double SystemProgress { get; set; }
    }
}
