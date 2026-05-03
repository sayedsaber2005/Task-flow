using ProjectManagement.BL.Interfaces;
using ProjectManagement.DTOs;
using ProjectManagement.DTOs.ProjectMembers;
using ProjectManagement.DTOs.Users;
using ProjectManagement.Models;
using ProjectManagement.Repositories.Interfaces;
using System.Security.Claims;

namespace ProjectManagement.BL.Implementations
{
    public class ClsProjectMembers : IProjectMembers
    {
        private readonly IProjectMemberRepository _repo;
        private readonly IHttpContextAccessor _httpContext;
        private readonly IUserRepository _userRepo;

        public ClsProjectMembers(
        IProjectMemberRepository repo,
        IHttpContextAccessor httpContext,
        IUserRepository userRepo)
        {
            _repo = repo;
            _httpContext = httpContext;
            _userRepo = userRepo;
        }

        public ApiResponse AddMember(AddMemberDTO dto)
        {
            var currentUserId =_httpContext.HttpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var member =_repo.GetByUserAndProject(currentUserId,dto.ProjectId);

            if (member == null)
            {
                return new ApiResponse
                {
                    Errors = new List<object>
                    {
                        new { Message = "Not a member of the project" }
                    },
                    StatusCode = "403"
                };
            }

            if (member.Role.ToLower() != "project manager")
            {
                return new ApiResponse
                {
                    Errors = new List<object>
                    {
                        new { Message = "Not authorized" }
                    },
                    StatusCode = "403"
                };
            }

            // 🔴 Get user by email
            var user =_userRepo.GetByEmail(dto.Email);

            if (user == null)
            {
                return new ApiResponse
                {
                    Errors = new List<object>
                    {
                        new { Message = "User not found" }
                    },
                    StatusCode = "404"
                };
            }

            // 🔴 Check if already member
            var existingMember =_repo.GetByUserAndProject(user.Id,dto.ProjectId);
            if (existingMember != null)
            {
                return new ApiResponse
                {
                    Errors = new List<object>
                    {
                        new { Message = "User already in project" }
                    },
                    StatusCode = "400"
                };
            }

            var newMember = new TbProjectMember
            {
                ProjectId = dto.ProjectId,
                UserId = user.Id,
                Role = dto.Role,
                JoinedAt = DateTime.UtcNow
            };

            _repo.Add(newMember);
            _repo.Save();

            return new ApiResponse
            {
                Data = new
                {
                    UserId = newMember.UserId,
                    Email = user.Email,
                    ProjectId = newMember.ProjectId,
                    Role = newMember.Role,
                    JoinedAt = newMember.JoinedAt
                },
                StatusCode = "201"
            };
        }

        public ApiResponse GetProjectMembers(int projectId)
        {
            var members = _repo.GetByProjectId(projectId);

            var result = members.Select(m => new ProjectMemberDTO
            {
                Id = m.Id,
                UserId = m.UserId,
                UserName = m.User?.FirstName + " " + m.User?.LastName,
                Role = m.Role
            });

            return new ApiResponse
            {
                Data = result,
                StatusCode = "200"
            };
        }

       public ApiResponse RemoveMember(int projectId, string userId)
        {
            var result = new ApiResponse();

            var currentUserId =_httpContext.HttpContext.User
                .FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var currentMember =_repo.GetByUserAndProject(currentUserId,projectId);

            if (currentMember == null)
            {
                result.StatusCode = "403";
                result.Errors.Add(new { Message = "Not a member of project" });
                return result;
            }

            if (currentMember.Role.ToLower() != "project manager")
            {
                result.StatusCode = "403";
                result.Errors.Add(new { Message = "Not authorized" });
                return result;
            }

            var memberToRemove =_repo.GetByUserAndProject(userId,projectId);

            if (memberToRemove == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { Message = "Member not found" });
                return result;
            }

            _repo.Delete(memberToRemove);
            _repo.Save();

            result.StatusCode = "200";

            return result;
        }

        public ApiResponse ChangeRole(ProjectRoleDTO dto)
        {
            var result = new ApiResponse();

            // Authorization check: Only project managers can change roles

            var user = _httpContext.HttpContext.User;
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            var projectManger = _repo.GetByUserAndProject(userId, dto.ProjectId);

            if (projectManger == null)
            {
                result.StatusCode = "404";
                return result;
            }

            if(projectManger.Role.ToLower() != "project manager")
            {
                result.Errors.Add(new { Message = "Not authorized" });
                result.StatusCode = "403";
                return result;
            }

            var member = _repo.GetByUserAndProject(dto.UserId, dto.ProjectId);

            if(member == null)
            {
                return new ApiResponse
                {
                    Errors = new List<object> { new { Message = "Member not found" } },
                    StatusCode = "404"
                };
            }

            member.Role = dto.Role;
            _repo.Save();

            result.StatusCode = "200";
            return result;
        }

        public ApiResponse SearchUserByEmail(SearchUserDTO dto)
        {
            var result = new ApiResponse();

            var user =_httpContext.HttpContext.User;

            var userId =user.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            // check PM permission
            var currentMember =_repo.GetByUserAndProject(userId,dto.ProjectId);

            if (currentMember == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new { Message = "Project not found" });
                return result;
            }

            if (currentMember.Role.ToLower()!= "project manager")
            {
                result.StatusCode = "403";
                result.Errors.Add(new { Message = "Not authorized" });
                return result;
            }

            var foundUser =_repo.SearchUserByEmailNotInProject(dto.Email,dto.ProjectId);
            if (foundUser == null)
            {
                result.StatusCode = "404";
                result.Errors.Add(new{Message ="User not found or already in project"});
                return result;
            }

            result.Data = foundUser;
            result.StatusCode = "200";

            return result;
        }
    }
}
