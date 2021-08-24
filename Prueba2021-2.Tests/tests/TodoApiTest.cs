using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using Prueba2021_2.Tests.Helpers;
using Xunit;
using Prueba2021_2.Functions.Functions;
using Prueba2021_2.Common.Models;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;

namespace Prueba2021_2.Tests.tests
{
    public class TodoApiTest
    {
        private readonly ILogger logger = TestFactory.CreateLogger();
        [Fact]
        public async void CreateTodo_Should_Rerturn_200()
        {
            // Arrege
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("Http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.GetTodoRequest();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoRequest);
            //Act
            IActionResult  response = await TodoApi.CreateTodo(request, mockTodos, logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);


        }

        [Fact]
        public async void UpdateTodo_Should_Rerturn_200()
        {
            // Arrege
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("Http://127.0.0.1:10002/devstoreaccount1/reports"));
            Todo todoRequest = TestFactory.GetTodoRequest();
            Guid todoId = Guid.NewGuid();
            DefaultHttpRequest request = TestFactory.CreateHttpRequest(todoId,todoRequest);
            //Act
            IActionResult response = await TodoApi.UpdateTodo(request,mockTodos,todoId.ToString(), logger);

            //Assert
            OkObjectResult result = (OkObjectResult)response;
            Assert.Equal(StatusCodes.Status200OK, result.StatusCode);


        }
    }
}
