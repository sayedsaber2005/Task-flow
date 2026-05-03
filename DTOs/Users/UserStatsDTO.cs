namespace ProjectManagement.DTOs.Users
{
    public class UserStatsDTO
    {
        public int TotalTasks { get; set; }
        public int DoneTasks { get; set; }
        public int InProgressTasks { get; set; }
        public int TodoTasks { get; set; }
        public int OverDueTasks { get; set; }
    }
}
