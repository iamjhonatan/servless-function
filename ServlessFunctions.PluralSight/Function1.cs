using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Azure;
using Azure.Data.Tables;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServlessFunctions.PluralSight;

public static class TodoApi
{
   [FunctionName("CreateTodo")]
   public static async Task<IActionResult> CreateTodo(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
      [Table("todos", Connection = "AzureWebJobsStorage")] IAsyncCollector<TodoTableEntity> todoTable, ILogger log)
   {
      log.LogInformation("Creating a new Todo list item");
      
      var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);
      var todo = new Todo { TaskDescription = input.TaskDescription };
      
      await todoTable.AddAsync(todo.ToTableEntity());
      
      return new OkObjectResult(todo);
   }

   [FunctionName("GetTodos")]
   public static async Task<IActionResult> GetTodos(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
      [Table("todos", Connection = "AzureWebJobsStorage")] TableClient todoTable, ILogger log)
   {
      log.LogInformation("Getting Todo list items");

      var page1 = await todoTable.QueryAsync<TodoTableEntity>().AsPages().FirstAsync();
      
      return new OkObjectResult(page1.Values.Select(Mappings.ToTodo));
   }

   [FunctionName("GetTodoById")]
   public static IActionResult GetTodoById(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
      [Table("todos", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoTableEntity todoTableEntity, ILogger log, string id)
   {
      log.LogInformation("Getting Todo item by id");

      if (todoTableEntity is null)
      {
         log.LogInformation($"Item {id} not found.");
         return new NotFoundResult();
      }

      return new OkObjectResult(todoTableEntity.ToTodo());
   }
   
   [FunctionName("UpdateTodo")]
   public static async Task<IActionResult> UpdateTodo(
      [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
      [Table("todos", Connection = "AzureWebJobsStorage")] TableClient todoTable, ILogger log, string id)
   {
      var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var updated = JsonConvert.DeserializeObject<TodoUpdateModel>(requestBody);
      TodoTableEntity existingRow;

      try
      {
         var findResult = await todoTable.GetEntityAsync<TodoTableEntity>("TODO", id);
         existingRow = findResult.Value;
      }
      catch (RequestFailedException e) when (e.Status == 404)
      {
         return new NotFoundResult();
      }

      existingRow.IsCompleted = updated.IsCompleted;

      if (!string.IsNullOrEmpty(updated.TaskDescription))
         existingRow.TaskDescription = updated.TaskDescription;
      
      await todoTable.UpdateEntityAsync(existingRow, existingRow.ETag, TableUpdateMode.Replace);

      return new OkObjectResult(existingRow.ToTodo());
   }

   [FunctionName("DeleteTodo")]
   public static async Task<IActionResult> DeleteTodo(
      [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
      [Table("todos", Connection = "AzureWebJobsStorage")] TableClient todoTable, ILogger log, string id)
   {
      try
      {
         await todoTable.DeleteEntityAsync("TODO", id, ETag.All);
      }
      catch (RequestFailedException e) when (e.Status == 404)
      {
         return new NotFoundResult();
      }

      return new OkResult();
   }
}