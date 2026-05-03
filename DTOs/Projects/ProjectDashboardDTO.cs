using ProjectManagement.DTOs.ProjectMembers;
using ProjectManagement.DTOs.Stats;
using ProjectManagement.DTOs.Tasks;

namespace ProjectManagement.DTOs.Projects
{
    public class ProjectDashboardDTO
    {
        public ProjectsDTO Project { get; set; }
        public List<TasksDTO> Tasks { get; set; }
        public List<ProjectMemberDTO> Members { get; set; }
        public ProjectStatsDTO Stats { get; set; }
    }
}
