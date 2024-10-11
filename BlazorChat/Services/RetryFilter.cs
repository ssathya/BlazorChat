using Microsoft.SemanticKernel;
using System.Net;

namespace BlazorChat.Services;

public class RetryFilter(string fallbackModelId) : IFunctionInvocationFilter
{
    public async Task OnFunctionInvocationAsync(FunctionInvocationContext context, Func<FunctionInvocationContext, Task> next)
    {
        try
        {
            // Try to invoke function
            await next(context);
        }
        // Catch specific exception
        catch (HttpOperationException exception) when (exception.StatusCode == HttpStatusCode.Unauthorized)
        {
            // Get current execution settings
            PromptExecutionSettings executionSettings = context.Arguments.ExecutionSettings![PromptExecutionSettings.DefaultServiceId];

            // Override settings with fallback model id
            executionSettings.ModelId = fallbackModelId;

            // Try to invoke function again
            await next(context);
        }
    }
}