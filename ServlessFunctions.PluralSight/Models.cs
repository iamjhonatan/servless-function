using System;

namespace ServlessFunctions.PluralSight;

public class Todo
{
    public string Id { get; set; } = Guid.NewGuid().ToString("n");
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    public string TaskDescription { get; set; }
    public bool IsCompleted { get; set; }
}

public class TodoCreateModel
{
    public string TaskDescription { get; set; }
}

public class TodoUpdateModel
{
    public string TaskDescription { get; set; }
    public bool IsCompleted { get; set; }
}