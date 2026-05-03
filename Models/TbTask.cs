using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ProjectManagement.Models;

public partial class TbTask
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } 

    public int? ProjectId { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public string? AssignedTo { get; set; }

    public string? CreatedBy { get; set; }

    public string? Priority { get; set; }

    public string? Status { get; set; }

    public DateTime? DueDate { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ApplicationUser? AssignedToNavigation { get; set; }

    public virtual ApplicationUser? CreatedByNavigation { get; set; }

    public virtual TbProject? Project { get; set; }

    public virtual ICollection<TbAttachment> TbAttachments { get; set; } = new List<TbAttachment>();

    public virtual ICollection<TbComment> TbComments { get; set; } = new List<TbComment>();

    public virtual ICollection<TbTaskHistory> TbTaskHistories { get; set; } = new List<TbTaskHistory>();
}
