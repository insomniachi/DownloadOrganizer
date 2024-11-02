using System.Diagnostics;
using System.Text.Json;
using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Server.Api;

#if !DEBUG
using Microsoft.AspNetCore.Hosting;
#endif

var builder = WebApplication.CreateBuilder(args);
#if DEBUG
builder.Configuration.AddUserSecrets<Program>();
#else
var confPath = builder.Configuration.GetValue<string>("ConfigPath");
if (File.Exists(confPath))
{
    builder.Configuration.AddJsonFile(confPath);
}
#endif

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Transient);
builder.Services.AddSystemd();
builder.Services.AddHttpClient();
builder.Services.AddTelegramBot();
builder.Services.AddTransient<JellyfinWebhook>();

#if !DEBUG
builder.WebHost.UseUrls("http://0.0.0.0:4000");
#endif

builder.Services
    .AddOptions<JellyfinSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Jellyfin"))
    .ValidateFluently()
    .ValidateOnStart();

builder.Services
    .AddOptions<TelegramSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Telegram"))
    .ValidateFluently()
    .ValidateOnStart();

var app = builder.Build();

app.MapPost("/Jellyfin/Webhook", async (HttpContext context, [FromServices]JellyfinWebhook handler) =>
{
    var jsonDocument = await JsonDocument.ParseAsync(context.Request.Body);
    var rootElement = jsonDocument.RootElement;
    var notificationType = rootElement.GetProperty("NotificationType").GetString();

    if (string.IsNullOrEmpty(notificationType))
    {
        return;
    }

    await (notificationType switch
    {
        "ItemAdded" => handler.HandleItemAdded(rootElement.Deserialize<ItemAdded>()),
        "AuthenticationSuccess" => handler.HandleAuthenticationSuccess(rootElement.Deserialize<UserAuthorized>()),
        "PlaybackStart" => handler.HandlerPlaybackStarted(rootElement.Deserialize<PlaybackStarted>()),
        "PlaybackStop" => handler.HandlerPlaybackStopped(rootElement.Deserialize<PlaybackStopped>()),
        "PluginUpdated" => handler.HandlePluginUpdated(rootElement.Deserialize<PluginInfo>()),
        "PluginInstalled" => handler.HandlePluginInstalled(rootElement.Deserialize<PluginInfo>()),
        _ => Task.CompletedTask
    });

});

app.MapPost("/vpn", (NordVpnOptions options) =>
{
    var args = options.Connect ? "connect" : "disconnect";
    
    var process = new Process
    {
        StartInfo = new ProcessStartInfo
        {
            FileName = "/usr/bin/nordvpn",
            Arguments = args
        }
    };

    process.Start();
});

app.Run();