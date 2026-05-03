using ProjectManagement.DTOs;
using ProjectManagement.DTOs.Projects;

namespace ProjectManagement.BL.Interfaces
{
    public interface IProject
    {
        ApiResponse GetAll();
        ApiResponse GetById(int id);
        ApiResponse GetMyProjects();
        ApiResponse GetProjectDetails(int projectId);
        ApiResponse GetProjectDashboard(int projectId);
        ApiResponse GetProjectStats(int projectId);
        ApiResponse Create(CreateProjectDTO dto);
        ApiResponse Update(UpdateProjectDTO dto);
        ApiResponse Delete(int id);
        ApiResponse GetProjectProgress(int projectId);
        ApiResponse GetTeamWorkload(int projectId);
    }
}