using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Azure.WebJobs.Host;
using Newtonsoft.Json;

namespace ServlessFunctions.PluralSight;

public static class TodoApi
{
   private static List<Todo> items = new List<Todo>();

   [FunctionName("CreateTodo")]
   public static async Task<IActionResult> CreateTodo(
      [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")]
      HttpRequest req, TraceWriter log)
   {
      log.Info("Creating a new Todo list item");
      string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
      var input = JsonConvert.DeserializeObject<TodoCreateModel>(requestBody);

      var todo = new Todo { TaskDescription = input.TaskDescription };
      items.Add(todo);
      return new OkObjectResult(todo);
   }
}