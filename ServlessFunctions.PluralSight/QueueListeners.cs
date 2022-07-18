using System.Threading.Tasks;
using Microsoft.Azure.Storage.Blob;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace ServlessFunctions.PluralSight;
    
public static class QueueListeners
{
    // TODO: adjust function to new packages
    
    [FunctionName("QueueListeners")]
    public static async Task Run([QueueTrigger("todos", Connection = "AzureWebJobsStorage")]Todo todo,
        [Blob("todos", Connection = "AzureWebJobsStorage")]CloudBlobContainer container,
        ILogger log)
    {
        await container.CreateIfNotExistsAsync();
        var blob = container.GetBlockBlobReference($"{todo.Id}.txt");
        await blob.UploadTextAsync($"Created a new task: {todo.TaskDescription}");
        log.LogInformation($"C# Queue trigger function processed: {todo.TaskDescription}");
    }
}
