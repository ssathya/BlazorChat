﻿using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
using Microsoft.SemanticKernel;
using System.Text;
using Markdig;

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

    protected override void OnInitialized()
    {
        History.AddSystemMessage("You are a friendly and jovial assistant who is not shy about asking for clarification. " +
            " When the user requests you for the price for a Ticker you will get stock quote for the ticker."+
            " Your response will be detailed and provide all information regarding the ticker's current quote.");
        settings = new()
        {
            ToolCallBehavior = ToolCallBehavior.AutoInvokeKernelFunctions
        };
    }
    protected async Task OnSubmitClick()
    {
        if (string.IsNullOrEmpty(UserInput))
        {
            Logger?.LogInformation("Not processing; no input from user");
            return;
        }
        History.AddUserMessage(UserInput);
        responseToDisplay.AppendLine(Markdown.ToHtml($"> *{UserInput}*\n\n"));
        StringBuilder tmpBuffer = new();
        var chunks = ChatCompletionService!.GetStreamingChatMessageContentsAsync(History, settings!, Kernel!);
        await foreach (var chunk in chunks)
        {
            tmpBuffer.Append(chunk);
        }
        responseToDisplay.Append(Markdown.ToHtml(tmpBuffer.ToString()));
        responseToDisplay.Append("\n<br/>");
        StateHasChanged();

        History.AddAssistantMessage(tmpBuffer.ToString());
    }
}