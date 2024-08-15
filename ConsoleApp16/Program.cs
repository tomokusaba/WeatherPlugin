using Azure.Identity;
using ConsoleApp16;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

string deploymentName = config["OpenAI:DeploymentName"] ?? throw new InvalidOperationException("OpenAI:DeploymentName is not set.");
string endpoint = config["OpenAI:Endpoint"] ?? throw new InvalidOperationException("OpenAI:BaseUrl is not set.");

var builder = Kernel.CreateBuilder();
builder.AddAzureOpenAIChatCompletion(
   deploymentName,
   endpoint,
   new DefaultAzureCredential()).Build();
builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
builder.Services.AddSingleton<HttpClient>();
builder.Plugins.AddFromType<WeatherPlugin>();
Kernel kernel = builder.Build();

OpenAIPromptExecutionSettings? setting = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,

};

while (true)
{
    Console.Write("User > ");
    string input = Console.ReadLine()!;
    if (input == "exit")
    {
        break;
    }
    else
    {
        var result = await kernel.InvokePromptAsync(input, new(setting));
        Console.WriteLine($"Assistant > {result}");
    }
}
