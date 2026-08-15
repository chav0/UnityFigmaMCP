using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using UnityFigmaMCP.Common;
using UnityFigmaMCP.Server.Figma;
using UnityFigmaMCP.Server.Unity;

namespace UnityFigmaMCP.Server
{
    internal static class Program
    {
        public static async Task Main(string[] args)
        {
            var ownsHub = IsPortAvailable(SignalRDefaults.HttpPort);

            var builder = WebApplication.CreateBuilder(args);

            builder.Logging.ClearProviders();
            builder.Logging.AddConsole(options => options.LogToStandardErrorThreshold = LogLevel.Trace);

            builder.WebHost.UseUrls(ownsHub ? SignalRDefaults.BaseUrl : "http://127.0.0.1:0");

            builder.Services.AddSignalR(options =>
            {
                options.MaximumReceiveMessageSize = 1024 * 1024;
            });

            builder.Services.AddSingleton<IFigmaApiClient>(_ => new FigmaApiClient(FigmaToken.Value));
            builder.Services.AddSingleton<IFigmaAuthService, FigmaAuthService>();
            builder.Services.AddSingleton<IFigmaFileService, FigmaFileService>();
            builder.Services.AddSingleton<IFigmaImageService, FigmaImageService>();

            builder.Services.AddSingleton<IUnityConnectionRegistry, UnityConnectionRegistry>();
            builder.Services.AddSingleton<IUnityCommandService, UnityCommandService>();

            builder.Services
                .AddMcpServer()
                .WithStdioServerTransport()
                .WithToolsFromAssembly();

            var app = builder.Build();

            if (ownsHub)
            {
                app.MapGet("/", () => "UnityFigma MCP — SignalR host. Hub: " + SignalRDefaults.HubUrl);
                app.MapHub<UnityMcpHub>(SignalRDefaults.HubPath);
            }
            else
            {
                app.Logger.LogInformation("Port {Port} is owned by another MCP server instance; Unity tools will route through that one", SignalRDefaults.HttpPort);
            }

            await app.RunAsync().ConfigureAwait(false);
        }

        private static bool IsPortAvailable(int port)
        {
            try
            {
                using var listener = new TcpListener(IPAddress.Loopback, port);

                listener.Start();
                listener.Stop();

                return true;
            }
            catch (SocketException)
            {
                return false;
            }
        }
    }
}
