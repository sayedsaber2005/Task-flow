using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbTaskHistory
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public string? ChangedBy { get; set; }

    public string? FieldChanged { get; set; }

    public string? OldValue { get; set; }

    public string? NewValue { get; set; }

    public DateTime? ChangedAt { get; set; }

    public virtual ApplicationUser? ChangedByNavigation { get; set; }

    public virtual TbTask? Task { get; set; }
}
