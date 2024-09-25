﻿using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.SemanticKernel;
using Microsoft.SemanticKernel.ChatCompletion;
using Microsoft.SemanticKernel.Connectors.OpenAI;
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

    private ChatHistory History = new();

    protected StringBuilder responseToDisplay = new();
    protected string UserInput = string.Empty;
    protected OpenAIPromptExecutionSettings? settings;

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