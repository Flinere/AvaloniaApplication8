using System;
using System.Collections.Generic;

namespace AvaloniaApplication8.Models;

public partial class Task
{
    public int Id { get; set; }

    public int? UserId { get; set; }

    public int? CategoryId { get; set; }

    public string Title { get; set; } = null!;

    public string? Description { get; set; }

    public string? Priority { get; set; }

    public string? Status { get; set; }

    public DateTime? Deadline { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public virtual Category? Category { get; set; }

    public virtual ICollection<TaskReminder> TaskReminders { get; set; } = new List<TaskReminder>();

    public virtual User? User { get; set; }
}
