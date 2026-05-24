using System;
using System.Collections.Generic;

namespace AvaloniaApplication8.Models;

public partial class TaskReminder
{
    public int Id { get; set; }

    public int? TaskId { get; set; }

    public DateTime RemindAt { get; set; }

    public bool? IsSent { get; set; }

    public string? Channel { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual Task? Task { get; set; }
}
