using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
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

builder.Services.AddValidatorsFromAssemblyContaining<Program>(ServiceLifetime.Transient);
builder.Services.AddSystemd();
builder.Services.AddHttpClient();
builder.Services.AddTelegramBot();

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

app.MapPost("/Jellyfin/Added", async (JellyfinAddedEvent item,
    [FromServices] ITelegramBotClient bot,
    [FromServices] IHttpClientFactory httpClientFactory,
    [FromServices] IOptions<TelegramSettings> telegramSettings,
    [FromServices] IOptions<JellyfinSettings> jellyfinSettings) =>
{
    await JellyfinWebhook.HandleItemAdded(item, bot, httpClientFactory, jellyfinSettings.Value, telegramSettings.Value);
});

app.Run();