using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace ServlessFunctions.PluralSight;

public static class ScheduledFunction
{
    [FunctionName("ScheduledFunction")]
    public static async Task RunAsync([TimerTrigger("0 */5 * * * *")] TimerInfo myTimer,
        [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable, ILogger log)
    {
        var query = new TableQuery<TodoTableEntity>();
        var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
        var deleted = 0;

        foreach (var todo in segment)
        {
            if (todo.IsCompleted)
            {
                await todoTable.ExecuteQuerySegmentedAsync(TableOperation.Delete(todo));
                deleted++;
            }
        }
        
        log.LogInformation($"Deleted {deleted} items at {DateTime.Now}");
    }
}