using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Polly;
using Polly.Retry;
using System.Text;

namespace BlazorChat.Components.Pages;

public partial class Chat
{
    [Inject]
    public ILogger<Chat>? Logger { get; set; }

    [Inject]
    public IChatCompletionService? ChatCompletionService { get; set; }

    [Inject]
    public Kernel? Kernel { get; set; }

    private ChatHistory History = [];

    protected StringBuilder responseToDisplay = new();
    protected string UserInput = string.Empty;
    protected OpenAIPromptExecutionSettings? settings;
    protected MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    protected override void OnInitialized()
    {
        History.AddSystemMessage("You are a friendly and jovial assistant who is not shy about asking for clarification. " +
            "You will be precise and keep your answers brief." +
            "However, if you are asked for a blog post, details, essay, or research paper on a topic, please provide " +
            "detailed information on the requested subject." +
            "If weather information is requested get weather for the provided city. If no city name is provided show" +
            "the help message to provide city name. " +
             "If the user would not give a news category get the list of news categories that we can source." +
            "If user retrieves news, ask if he/she want to save it and get the file name.");
        settings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
    }

    protected void OnClearUserInput()
    {
        UserInput = string.Empty;
        StateHasChanged();
    }

    protected async Task OnSubmitClick()
    {
        if (string.IsNullOrEmpty(UserInput))
        {
            Logger?.LogInformation("Not processing; no input from user");
            return;
        }
        History.AddUserMessage(UserInput);
        responseToDisplay.Append("""<div class="text-end badge text-bg-primary text-wrap"> """);
        responseToDisplay.AppendLine(Markdown.ToHtml($"> *{UserInput}*\n\n"));
        responseToDisplay.Append("</div>");
        StringBuilder tmpBuffer = new();
        try
        {
            var chunks = await retryPolicy.ExecuteAsync(() =>
            {
                return Task.FromResult(ChatCompletionService!.GetStreamingChatMessageContentsAsync(History, settings!, Kernel!));
            });
            await foreach (var chunk in chunks)
            {
                tmpBuffer.Append(chunk);
            }
            if (ChatCompletionService!.Attributes.ContainsKey("Usage"))
            {
                responseToDisplay.Append($"<br/>Usage: {ChatCompletionService.Attributes["Usage"]}");
            }
            responseToDisplay.Append(Markdown.ToHtml(tmpBuffer.ToString(), pipeline));
            responseToDisplay.Append("\n<br/>");
        }
        catch (Exception ex)
        {
            Logger?.LogError(ex, "Error processing user input");
            responseToDisplay.Append("/n<p>Unexpected error occurred. Please try again later.</p>\n<br/>");
        }
        StateHasChanged();

        History.AddAssistantMessage(tmpBuffer.ToString());
    }

    private static readonly AsyncRetryPolicy retryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(
            retryCount: 3, // Number of retries
            sleepDurationProvider: _ => TimeSpan.FromSeconds(40), // Wait time between retries
            onRetry: (exception, timeSpan, retryCount, context) =>
            {
                // Optional: Log the retry attempt
                Console.WriteLine($"Retry {retryCount} after {timeSpan.Seconds} seconds due to: {exception.Message}");
            });
}