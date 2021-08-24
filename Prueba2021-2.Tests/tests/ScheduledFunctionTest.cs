using Prueba2021_2.Functions.Functions;
using Prueba2021_2.Tests.Helpers;
using System;
using Xunit;

namespace Prueba2021_2.Tests.tests
{
    public class ScheduledFunctionTest
    {
        [Fact]
        public void ScheduledFunction_Should_Log_Message()
        {
            //Arrange
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("Http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.list);
            //Act
            ScheduledFunction.Run(null, mockTodos, logger);
            string message = logger.Logs[0];

            //Assert
            Assert.Contains("Deleting completed", message);
        }

    }
}
