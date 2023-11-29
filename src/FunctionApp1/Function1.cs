namespace FunctionApp1
{
    using Azure.Storage.Queues.Models;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask.Client;
    using Microsoft.Extensions.Logging;
    using System.Threading.Tasks;

    public class Function1
    {
        private readonly ILogger<Function1> logger;

        public Function1(ILogger<Function1> logger)
        {
            this.logger = logger;
        }

        [Function(nameof(Function1))]
        public async Task RunAsync(
            [QueueTrigger("myqueue-items", Connection = "myconnectionstring")] QueueMessage message,
            [DurableClient] DurableTaskClient client)
        {
            this.logger.LogInformation($"C# Queue trigger function processed: {message.MessageText}");

            // Function input comes from the request content.
            string instanceId = await client.ScheduleNewOrchestrationInstanceAsync(nameof(Orchestrator1));

            this.logger.LogInformation("Started orchestration with ID = '{instanceId}'.", instanceId);
        }
    }
}
