using System;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;

namespace ServlessFunctions.PluralSight;


// Function with packages deprecated
public static class ScheduledFunction
{
    // TODO: adjust function to new packages
    
    [FunctionName("ScheduledFunction")]
    public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer,
        [Table("todos", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
        ILogger log)
    {
        var query = new TableQuery();
        var segment = await todoTable.ExecuteQuerySegmentedAsync(query, null);
        var deleted = 0;
        
        foreach (var todo in segment)
        {
            //if (todo.IsCompleted)
            {
                await todoTable.ExecuteAsync(TableOperation.Delete(todo));
                deleted++;
            }
        }
        
        log.LogInformation($"Deleted {deleted} items at {DateTime.Now}");
    }
}