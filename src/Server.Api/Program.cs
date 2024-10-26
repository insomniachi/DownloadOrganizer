using Microsoft.AspNetCore.Mvc;
using Server.Api;

#if !DEBUG
using Microsoft.AspNetCore.Hosting;
#endif
using Telegram.Bot;

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

builder.Services.AddSystemd();
builder.Services.AddHttpClient();
builder.Services.AddTelegramBot();

#if !DEBUG
builder.WebHost.UseUrls("http://0.0.0.0:4000");
#endif


var app = builder.Build();

app.MapPost("/Jellyfin/Added", async (JellyfinAddedEvent item,
    [FromServices] ITelegramBotClient bot,
    [FromServices] IHttpClientFactory httpClientFactory,
    [FromServices] IConfiguration configuration) =>
{
    await JellyfinWebhook.HandleItemAdded(item, bot, httpClientFactory, configuration);
});

app.Run();