namespace FunctionApp1
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask;
    using Microsoft.Extensions.Logging;

    public class Orchestrator2
    {
        private readonly ILogger<Orchestrator1> logger;

        public Orchestrator2(ILogger<Orchestrator1> logger)
        {
            this.logger = logger;
        }

        [Function(nameof(Orchestrator2))]
        public async Task<Guid> RunAsync(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(Orchestrator1));
            logger.LogInformation("Saying hello2.");
            var outputs = new List<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello2), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello2), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(SayHello2), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return Guid.NewGuid();
        }

        [Function(nameof(SayHello2))]
        public string SayHello2([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SayHello2");
            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }
    }
}
