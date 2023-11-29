namespace FunctionApp1
{
    using Microsoft.Azure.Functions.Worker;
    using Microsoft.DurableTask;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;

    public class Orchestrator1
    {
        private readonly ILogger<Orchestrator1> logger;

        public Orchestrator1(ILogger<Orchestrator1> logger)
        {
            this.logger = logger;
        }

        [Function(nameof(Orchestrator1))]
        public async Task<Guid> RunAsync(
            [OrchestrationTrigger] TaskOrchestrationContext context)
        {
            ILogger logger = context.CreateReplaySafeLogger(nameof(Orchestrator1));
            logger.LogInformation("Saying hello.");
            var outputs = new List<string>();

            // Replace name and input with values relevant for your Durable Functions Activity
            outputs.Add(await context.CallActivityAsync<string>(nameof(this.SayHello), "Tokyo"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(this.SayHello), "Seattle"));
            outputs.Add(await context.CallActivityAsync<string>(nameof(this.SayHello), "London"));

            // returns ["Hello Tokyo!", "Hello Seattle!", "Hello London!"]
            return Guid.NewGuid();
        }

        [Function(nameof(SayHello))]
        public string SayHello([ActivityTrigger] string name, FunctionContext executionContext)
        {
            ILogger logger = executionContext.GetLogger("SayHello");
            logger.LogInformation("Saying hello to {name}.", name);
            return $"Hello {name}!";
        }
    }
}
