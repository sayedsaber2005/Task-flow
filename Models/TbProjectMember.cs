using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbProjectMember
{
    public int Id { get; set; }

    public int? ProjectId { get; set; }

    public string? UserId { get; set; }

    public string? Role { get; set; }

    public DateTime? JoinedAt { get; set; }

    public virtual TbProject? Project { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
