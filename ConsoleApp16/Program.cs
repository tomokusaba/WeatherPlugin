using Azure.Identity;
using ConsoleApp16;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
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
   new DefaultAzureCredential());

builder.Services.AddLogging(c => c.AddDebug().SetMinimumLevel(LogLevel.Trace));
builder.Services.AddSingleton<HttpClient>();
builder.Plugins.AddFromType<WeatherPlugin>();
Kernel kernel = builder.Build();

#pragma warning disable SKEXP0040 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。
var prompty = kernel.CreateFunctionFromPromptyFile("weather.prompty");
#pragma warning restore SKEXP0040 // 種類は、評価の目的でのみ提供されています。将来の更新で変更または削除されることがあります。続行するには、この診断を非表示にします。

ChatHistory chatHistory = new();
var chat = kernel.GetRequiredService<IChatCompletionService>();

OpenAIPromptExecutionSettings? setting = new()
{
    ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
    MaxTokens = 2000,
};
Dictionary<string, object> extensionData = new()
{
    ["MaxTokens"] = 2000,
    ["Temperature"] = 1,
};
PromptExecutionSettings promptExecutionSettings = new()
{
    FunctionChoiceBehavior = FunctionChoiceBehavior.Auto(),
    //ExtensionData = extensionData
};

while (true)
{
    Console.Write("User > ");
    string input = Console.ReadLine()!;

    //chatHistory.AddUserMessage(input);

    if (string.IsNullOrEmpty(input))
    {
        break;
    }
    else
    {
        //var result = await kernel.InvokePromptAsync(input, new(setting));
        //var result = await chat.GetChatMessageContentAsync(chatHistory, promptExecutionSettings, kernel);
        //chatHistory.AddAssistantMessage(result.ToString());
        var result = await prompty.InvokeAsync(kernel, new KernelArguments(promptExecutionSettings)
        {
            ["question"] = input
        });
        Console.WriteLine($"Assistant > {result}");
    }
}
