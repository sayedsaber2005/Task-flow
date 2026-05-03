using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbNotification
{
    public int Id { get; set; }

    public string? UserId { get; set; }

    public string? Type { get; set; }

    public string? Message { get; set; }

    public bool? IsRead { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ApplicationUser? User { get; set; }
}
