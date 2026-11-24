/*
===========================================================================================================================
Building a custom local MCP server with local Downloads folder inspection tools over standard input/output (stdio).
 
Note: As intended to showcase the flow, there is no error handling, logging, or any other production-ready features in this demo.
===========================================================================================================================
*/

using System.ComponentModel;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

var builder = Host.CreateApplicationBuilder(args);

// Disable console logging to stdout so log messages don't corrupt the JSON-RPC stream.
builder.Logging.ClearProviders();

// Register the MCP server and the custom tools in DI container
builder.Services
    .AddMcpServer()
    .WithStdioServerTransport()
    .WithToolsFromAssembly();

// Build and run the application, which starts the MCP server and waits for incoming requests from the agent.
await builder.Build().RunAsync();

//bCreating custom tools for the MCP server
[McpServerToolType]
public static class DownloadsTools
{
    [McpServerTool]
    [Description("Counts the number of visible files in the user's Downloads folder.")]
    public static int CountDownloadedFiles()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        if (!Directory.Exists(path)) return 0;

        return Directory.EnumerateFiles(path)
            .Count(f => !Path.GetFileName(f).StartsWith('.') && Path.GetFileName(f) != "desktop.ini");
    }

    [McpServerTool]
    [Description("Calculates the total size of visible files in the user's Downloads folder in megabytes.")]
    public static double GetDownloadsSizeMb()
    {
        string path = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

        if (!Directory.Exists(path)) return 0;

        long totalBytes = Directory.EnumerateFiles(path)
            .Where(f => !Path.GetFileName(f).StartsWith('.') && Path.GetFileName(f) != "desktop.ini")
            .Sum(f => new FileInfo(f).Length);

        return Math.Round(totalBytes / (1024.0 * 1024.0), 2);
    }
}
