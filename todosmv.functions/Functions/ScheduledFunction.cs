using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using System;
using System.Threading.Tasks;
using todosmv.functions.Entities;

namespace todosmv.functions.Functions
{
    public static class ScheduledFunction
    {
        [FunctionName("ScheduledFunction")]
        public static async Task Run(
            [TimerTrigger("* * * * *")] TimerInfo myTimer,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation($"Deleting completed function executed at: {DateTime.Now}");
            string filter = TableQuery.GenerateFilterConditionForBool("IsCompleted", QueryComparisons.Equal, true);
            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>().Where(filter);
            TableQuerySegment<TodoEntity> completeTodos = await todoTable.ExecuteQuerySegmentedAsync(query, null);
            int deleted = 0;
            foreach (TodoEntity completeTodo in completeTodos)
            {
                await todoTable.ExecuteAsync(TableOperation.Delete(completeTodo));
                deleted++;
            }
            log.LogInformation($"Deleted {deleted} todo at: {DateTime.Now}");
        }
    }
}
