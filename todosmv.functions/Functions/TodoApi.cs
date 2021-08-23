using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using System.IO;
using System.Threading.Tasks;
using todosmv.Common.models;
using todosmv.Common.responses;
using todosmv.functions.Entities;

namespace todosmv.functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Recived a new Todo");

            string name = req.Query["name"];

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request body must have a TaskDescription"
                });
            }

            TodoEntity todoEntity = new TodoEntity
            {
                CreatedTime = System.DateTime.UtcNow,
                ETag = "*",
                IsCompleted = false,
                PartitionKey = "TODO",
                RowKey = System.Guid.NewGuid().ToString(),
                TaskDescription = todo.TaskDescription

            };

            TableOperation addOperation = TableOperation.Insert(todoEntity);
            await todoTable.ExecuteAsync(addOperation);

            string message = "New todo stored in table";
            log.LogInformation(message);



            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity

            });
        }
    }
}
