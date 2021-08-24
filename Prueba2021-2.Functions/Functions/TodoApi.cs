using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Microsoft.WindowsAzure.Storage.Table;
using Newtonsoft.Json;
using Prueba2021_2.Common.Models;
using Prueba2021_2.Common.Models.Responses;
using Prueba2021_2.Functions.Entities;
using System;
using System.IO;
using System.Threading.Tasks;

namespace Prueba2021_2.Functions.Functions
{
    public static class TodoApi
    {
        [FunctionName(nameof(CreateTodo))]//tabla crear
        public static async Task<IActionResult> CreateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("Recived a new todo.");
           
            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

            if (string.IsNullOrEmpty(todo?.TaskDescription))
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "The request must have a TaskDescription. /el requerimiento debe tener una TaskDesprip..."
                });
            }
           TodoEntity todoEntity = new TodoEntity
                {
                    createdTime = DateTime.UtcNow,
                    ETag = "*",
                    IsCompleted = false,
                    PartitionKey = "TODO",
                    RowKey = Guid.NewGuid().ToString(),
                    TaskDescription = todo.TaskDescription,

                };

                TableOperation addOperation = TableOperation.Insert(todoEntity);
                await todoTable.ExecuteAsync(addOperation);
                string message = "new todo Stored in table. Almacenado en la Tabla";
                log.LogInformation(message);


                return new OkObjectResult(new Response
                {
                    IsSuccess = true,
                    Message = message,
                    Result = todoEntity
                });
            
            
        }

        [FunctionName(nameof(UpdateTodo))]//tabla actualizar 
        public static async Task<IActionResult> UpdateTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "put", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
    {
        log.LogInformation($"Update for todo: {id}, received."); //id  recivido 

        string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
        Todo todo = JsonConvert.DeserializeObject<Todo>(requestBody);

        // Validate todo ID
        TableOperation findOperation = TableOperation.Retrieve<TodoEntity>("TODO", id);
        TableResult findResult = await todoTable.ExecuteAsync(findOperation);
        if (findResult.Result == null)
        {
            return new BadRequestObjectResult(new Response
            {
                IsSuccess = false,
                Message = "Todo not found."
            });
        }
        //update todo - Actualizar el todoo
        TodoEntity todoEntity = (TodoEntity)findResult.Result;
        todoEntity.IsCompleted = todo.IsCompleted;

        if (!string.IsNullOrEmpty(todo.TaskDescription))
        {
            todoEntity.TaskDescription = todo.TaskDescription; //Actualizar la descripcion

        }

        TableOperation addOperation = TableOperation.Replace(todoEntity);
        await todoTable.ExecuteAsync(addOperation);

        string message = $"Todo: {id}, Update in table";
        log.LogInformation(message);

        return new OkObjectResult(new Response
        {
            IsSuccess = true,
            Message = message,
            Result = todoEntity
        });

    }
      
        [FunctionName(nameof(GetAllTodos))]//todos los  todos
        public static async Task<IActionResult> GetAllTodos(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo")] HttpRequest req,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            ILogger log)
        {
            log.LogInformation("RGet all todos Recevied.");

            TableQuery<TodoEntity> query = new TableQuery<TodoEntity>();
            TableQuerySegment<TodoEntity> todos = await todoTable.ExecuteQuerySegmentedAsync(query, null);

            string message = "Retrieved all todos.";
            log.LogInformation(message);


            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todos
            });


        }

        [FunctionName(nameof(GetTodoById))]//Filtrar
        public static IActionResult GetTodoById(
            [HttpTrigger(AuthorizationLevel.Anonymous, "get", Route = "todo/{id}")] HttpRequest req,
            [Table("todo","TODO","{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
            string id,
            ILogger log)
        {
            log.LogInformation($"Get todo by id: {id}, Recevied.");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }

            string message = $"Todo: {todoEntity.RowKey}, retrieved.";
            log.LogInformation(message);

            return new OkObjectResult(new Response
            {
                IsSuccess = true,
                Message = message,
                Result = todoEntity
            });


        }

        [FunctionName(nameof(DeleteTodo))]//Borrar Tarea
        public static async Task<IActionResult> DeleteTodo(
            [HttpTrigger(AuthorizationLevel.Anonymous, "delete", Route = "todo/{id}")] HttpRequest req,
            [Table("todo", "TODO", "{id}", Connection = "AzureWebJobsStorage")] TodoEntity todoEntity,
            [Table("todo", Connection = "AzureWebJobsStorage")] CloudTable todoTable,
            string id,
            ILogger log)
    {
            log.LogInformation($"Get todo by id: {id}, Recevied.");

            if (todoEntity == null)
            {
                return new BadRequestObjectResult(new Response
                {
                    IsSuccess = false,
                    Message = "Todo not found."
                });
            }
            await todoTable.ExecuteAsync(TableOperation.Delete(todoEntity));
            string message = $"Todo: {todoEntity.RowKey}, delete.";
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
