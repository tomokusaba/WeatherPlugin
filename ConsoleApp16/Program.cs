using Azure.Identity;
using ConsoleApp16;
using Microsoft.Extensions.Configuration;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;

IConfigurationRoot config = new ConfigurationBuilder()
    .AddEnvironmentVariables()
    .AddUserSecrets<Program>()
    .Build();

string deploymentName = config["OpenAI:DeploymentName"] ?? throw new InvalidOperationException("OpenAI:DeploymentName is not set.");
//string modelId = config["OpenAI:ModelId"] ?? throw new InvalidOperationException("OpenAI:ModelId is not set.");
string endpoint = config["OpenAI:Endpoint"] ?? throw new InvalidOperationException("OpenAI:BaseUrl is not set.");
//string key = config["OpenAI:Key"] ?? throw new InvalidOperationException("OpenAI:Key is not set.");

Kernel kernel = Kernel.CreateBuilder()
 .AddAzureOpenAIChatCompletion(
    deploymentName,
    endpoint,
    new DefaultAzureCredential()).Build();

kernel.Plugins.AddFromType<WeatherPlugin>();

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
