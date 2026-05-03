namespace ProjectManagement.DTOs.Stats
{
    public class ProjectStatsDTO
    {
        public int TotalTasks { get; set; }
        public int DoneTasks { get; set; }
        public int TodoTasks { get; set; }
        public int OverDueTasks { get; set; }
        public int InProgressTasks { get; set; }
        public double ProgressPercentage { get; set; }
    }
}   
