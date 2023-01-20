using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Context.Features;
using Microsoft.Azure.Functions.Worker.Core.FunctionMetadata;
using Microsoft.Azure.Functions.Worker.Http;
using Microsoft.Azure.Functions.Worker.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System.Collections.Immutable;
using System.Text.Json;

namespace FunctionApp26
{
    internal partial class DirectFunctionExecutor : IFunctionExecutor
    {
        private readonly ILogger<DirectFunctionExecutor> _logger;

        public DirectFunctionExecutor(ILogger<DirectFunctionExecutor> logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task ExecuteAsync(FunctionContext context)
        {
            if (string.Equals(context.FunctionDefinition.Name, "Function1", StringComparison.OrdinalIgnoreCase))
            {
                var loggerFactory = context.InstanceServices.GetService<ILoggerFactory>()!;
                var modelBindingFeature = context.Features.Get<IModelBindingFeature>()!;

                var inputArguments = await modelBindingFeature.BindFunctionInputAsync(context);

                var target = new Function1(loggerFactory);

                var result = context.GetInvocationResult();
                result.Value = target.Run((HttpRequestData)inputArguments[0]!);
            }
        }
    }

    public class CustomFunctionMetadataProvider : IFunctionMetadataProvider
    {
        public Task<ImmutableArray<IFunctionMetadata>> GetFunctionMetadataAsync(string directory)
        {
            var metadataList = new List<IFunctionMetadata>();
            var Function0RawBindings = new List<string>();

            var Function0binding0 = new
            {
                Name = "req",
                Type = "HttpTrigger",
                Direction = "In",
                AuthLevel = (AuthorizationLevel)0,
                Methods = new List<string> { "get", "post" },
            };

            var Function0binding0JSON = "{\"name\":\"req\",\"type\":\"HttpTrigger\",\"direction\":\"In\",\"authLevel\":0,\"methods\":[\"get\",\"post\"]}";

            Function0RawBindings.Add(Function0binding0JSON);
            var Function0binding1 = new
            {
                Name = "$return",
                Type = "http",
                Direction = "Out",
            };

            var Function0binding1JSON = "{\"name\":\"$return\",\"type\":\"http\",\"direction\":\"Out\"}";

            Function0RawBindings.Add(Function0binding1JSON);
            var Function0 = new DefaultFunctionMetadata
            {
                Language = "dotnet-isolated",
                Name = "Function1",
                EntryPoint = "FunctionApp26.Function1.Run",
                RawBindings = Function0RawBindings,
                ScriptFile = "FunctionApp26.dll"
            };
            metadataList.Add(Function0);
            return Task.FromResult(metadataList.ToImmutableArray());
        }
    }

    public static class WorkerHostBuilderFunctionMetadataProviderExtension
    {
        ///<summary>
        /// Adds the GeneratedFunctionMetadataProvider to the service collection.
        /// During initialization, the worker will return generated function metadata instead of relying on the Azure Functions host for function indexing.
        ///</summary>
        public static IHostBuilder ConfigureGeneratedFunctionMetadataProvider(this IHostBuilder builder)
        {
            builder.ConfigureServices(s =>
            {
                s.AddSingleton<IFunctionMetadataProvider, CustomFunctionMetadataProvider>();
            });
            return builder;
        }
    }

}
