namespace ProjectManagement.DTOs.ProjectMembers
{
    public class AddMemberDTO
        {
            public int ProjectId { get; set; }

            public string Email { get; set; }

            public string Role { get; set; }
        }
}
