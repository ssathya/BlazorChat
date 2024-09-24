using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace BlazorChat.Services;
public class ArchivePlugin
{
    [KernelFunction("archive_data"), Description("Save data to a file on your computer.")]
    public async Task WriteData(Kernel kernel, string fileName, string data)
    {
        await File.WriteAllTextAsync(fileName, data);
    }
}
