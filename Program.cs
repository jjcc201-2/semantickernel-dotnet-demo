using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Kernel = Microsoft.SemanticKernel.Kernel;
using Microsoft.SemanticKernel.ChatCompletion;

using Plugins;

var builder = Kernel.CreateBuilder();

// Adding the native plugins to the kernel to expose the functions to the AI
builder.Plugins.AddFromType<ProductPlugin>();
builder.Plugins.AddFromType<LightPlugin>();

// Configure AI service credentials used by the kernel
var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();

// Depending on the settings.json file, use either Azure OpenAI or OpenAI as the desired LLM
if (useAzureOpenAI)
    builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
else
    builder.AddOpenAIChatCompletion(model, apiKey, orgId);

var kernel = builder.Build();

ChatHistory history = new();

var chatCompletionService = kernel.GetRequiredService<IChatCompletionService>();



// Start the conversation
Console.Write("User > ");

while (true)
{
    var userInput = Console.ReadLine();

    // Add the message from the agent to the chat history
    #pragma warning disable CS8604
    history.AddUserMessage(userInput);

    // Enable auto function calling
    OpenAIPromptExecutionSettings settings = new()
    {
        ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
    };

    
    // Get the response from the AI
    var result = chatCompletionService.GetStreamingChatMessageContentsAsync(
                        history,
                        executionSettings: settings,
                        kernel: kernel);

    // Stream the results 
    string fullMessage = "";
    var first = true;

    await foreach (var content in result)
    {
        if (content.Role.HasValue && first)
        {
            Console.Write("Assistant > ");
            first = false;
        }
        // Write the content of the result to the console
        Console.Write(content.Content);
        // Add the content of the result to the full message
        fullMessage += content.Content;
    }
    Console.WriteLine("\n");
    // Add the message from the agent to the chat history

    history.AddAssistantMessage(fullMessage);

    Console.Write("User > ");
}
