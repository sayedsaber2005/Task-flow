using ProjectManagement.DTOs.Projects;
using ProjectManagement.DTOs.Tasks;

namespace ProjectManagement.DTOs.Users
{
    public class UserDashboardDTO
    {
        public List<TasksDTO> MyTasks { get; set; }
        public List<ProjectsDTO> MyProjects { get; set; }
        public UserStatsDTO Stats { get; set; }
    }
}
