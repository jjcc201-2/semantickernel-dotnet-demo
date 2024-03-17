using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel.TemplateEngine;
using Kernel = Microsoft.SemanticKernel.Kernel;

var builder = Kernel.CreateBuilder();

// Configure AI service credentials used by the kernel
var (useAzureOpenAI, model, azureEndpoint, apiKey, orgId) = Settings.LoadFromFile();

// Depending on the settings.json file, use either Azure OpenAI or OpenAI as the desired LLM
if (useAzureOpenAI)
    builder.AddAzureOpenAIChatCompletion(model, azureEndpoint, apiKey);
else
    builder.AddOpenAIChatCompletion(model, apiKey, orgId);

var kernel = builder.Build();


// Example of an in-line semantic function
const string skPrompt = @"
ChatBot can have a conversation with you about any topic.
The ChatBot will be able to answer questions, give advice, and carry on a conversation.
The ChatBot will provide every response as a pirate. 
The ChatBot will also constantly remind you that it really thinks you should stay hydrated.
It can give explicit instructions or say 'I don't know' if it does not have an answer.


{{$history}}
User: {{$userInput}}
ChatBot:";

var executionSettings = new OpenAIPromptExecutionSettings 
{
    MaxTokens = 2000, // Defines the maximum number of tokens to generate
    Temperature = 0.7, // Defines the creativity and risk of answer. 0 is the most conservative & 1 is the most creative
    TopP = 0.5 // Another sampling parameter that can be used to control what tokens are considered when generating responses
};

var chatFunction = kernel.CreateFunctionFromPrompt(skPrompt, executionSettings);

var history = "";
var arguments = new KernelArguments()
{
    ["history"] = history
};


// Keep the conversation going on for eternity
while (true)
{
    Console.Write("You: ");
    var userInput = Console.ReadLine();

    // Add the user input to the arguments
    arguments["userInput"] = userInput;

    // Get the bot's answer by invoking the chat function
    var bot_answer = await chatFunction.InvokeAsync(kernel, arguments);

    // Add the bot's answer to the history
    history += $"\nUser: {userInput}\nAI: {bot_answer}\n";
    arguments["history"] = history;

    Console.WriteLine(history);
}
