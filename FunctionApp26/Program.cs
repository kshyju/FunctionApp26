using FunctionApp26;
using Microsoft.Azure.Functions.Core;
using Microsoft.Azure.Functions.Worker.Invocation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var host = new HostBuilder()
    .ConfigureFunctionsWorkerDefaults()
    .ConfigureServices(s =>
    {
        s.AddSingleton<IFunctionExecutor, DirectFunctionExecutor>();
        s.AddSingleton<IFunctionMetadataProvider, CustomFunctionMetadataProvider>();
    })
    .Build();

host.Run();
