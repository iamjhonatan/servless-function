using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace ServlessFunctions.PluralSight;

public static class TodoApi
{
   private static List<Todo> items = new List<Todo>();

   [FunctionName("CreateTodo")]
   public static async Task<IActionResult> CreateTodo(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]
      HttpRequest req, ILogger log)
   {
      log.LogInformation("Creating a new Todo list item");
      var requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

      var todo = new Todo { TaskDescription = input.TaskDescription };
      items.Add(todo);
      return new OkObjectResult(todo);
   }

   [FunctionName("GetTodos")]
   public static IActionResult GetTodos(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")]
      HttpRequest req, ILogger log)
   {
      log.LogInformation("Getting Todo list items");
      return new OkObjectResult(items);
   }

   [FunctionName("GetTodoById")]
   public static IActionResult GetTodoById(
      [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")]
      HttpRequest req, ILogger log, string id)
   {
      var todo = items.FirstOrDefault(t => t.Id == id);
      if (todo is null) 
         return new NotFoundResult();

      return new OkObjectResult(todo);
   }
}