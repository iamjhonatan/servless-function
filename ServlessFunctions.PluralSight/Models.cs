using System;
using Azure;
using Microsoft.WindowsAzure.Storage.Table;
using ITableEntity = Azure.Data.Tables.ITableEntity;

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

public class TodoTableEntity : BaseTableEntity
{
    public DateTime CreatedAt { get; set; }
    public string TaskDescription { get; set; }
    public bool IsCompleted { get; set; }
}

public class BaseTableEntity : ITableEntity
{
    public string PartitionKey { get; set; }
    public string RowKey { get; set; }
    public DateTimeOffset? Timestamp { get; set; }
    public ETag ETag { get; set; }
}

public static class Mappings
{
    public static TodoTableEntity ToTableEntity(this Todo todo)
    {
        return new TodoTableEntity
        {
            PartitionKey = "TODO",
            RowKey = todo.Id,
            CreatedAt = todo.CreatedAt,
            IsCompleted = todo.IsCompleted,
            TaskDescription = todo.TaskDescription
        };
    }

    public static Todo ToTodo(this TodoTableEntity todo)
    {
        return new Todo
        {
            Id = todo.RowKey,
            CreatedAt = todo.CreatedAt,
            IsCompleted = todo.IsCompleted,
            TaskDescription = todo.TaskDescription
        };
    }
}