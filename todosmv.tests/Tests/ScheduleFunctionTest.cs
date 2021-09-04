using System;
using todosmv.functions.Functions;
using todosmv.tests.Helpers;
using Xunit;

namespace todosmv.tests.Tests
{
    public class ScheduleFunctionTest
    {

        [Fact]
        public void ScheduleFunction_Should_Log_Message()
        {
            //Arrang 
            MockCloudTableTodos mockTodos = new MockCloudTableTodos(new Uri("http://127.0.0.1:10002/devstoreaccount1/reports"));
            ListLogger logger = (ListLogger)TestFactory.CreateLogger(LoggerTypes.List);

            //Act
            ScheduledFunction.Run(null, mockTodos, logger);
            string message = logger.Logs[0];

            //Assert

            Assert.Contains("Deleting completed", message);
        }
    }
}
