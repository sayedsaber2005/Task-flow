using System;
using System.Collections.Generic;

namespace ProjectManagement.Models;

public partial class TbAttachment
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public string? FileUrl { get; set; }

    public string? UploadedBy { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual TbTask? Task { get; set; }

    public virtual ApplicationUser? UploadedByNavigation { get; set; }
}
