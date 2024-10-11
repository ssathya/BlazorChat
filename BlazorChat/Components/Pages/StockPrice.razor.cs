using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Polly;
using Polly.Retry;
using System.Text;

namespace BlazorChat.Components.Pages;

public partial class StockPrice
{
    [Inject]
    public ILogger<Chat>? Logger { get; set; }

    [Inject]
    public IChatCompletionService? ChatCompletionService { get; set; }

    [Inject]
    public Kernel? Kernel { get; set; }

    private ChatHistory History = new();

    protected StringBuilder responseToDisplay = new();
    protected string UserInput = string.Empty;
    protected OpenAIPromptExecutionSettings? settings;
    protected MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();

    protected override void OnInitialized()
    {
        History.AddSystemMessage("You are a friendly and jovial assistant who is not shy about asking for clarification. " +
            " When the user requests you for the price for a Ticker you will get stock quote for the ticker." +
            " Your response will be detailed and provide all information regarding the ticker's current quote." +
            " If asked to convert currency you will identify the target currency, base currency, and amount to " +
            " convert and perform conversion");
        settings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions,
            Temperature = 0.2,
            TopP = 0.4
        };
        if (ChatCompletionService != null)
        {
        }
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