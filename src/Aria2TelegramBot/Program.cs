using Aria2NET;
using Aria2TelegramBot;
using FluentValidation;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);
#if DEBUG
builder.Configuration.AddUserSecrets<Program>();
#else
var confPath = builder.Configuration.GetValue<string>("ConfigPath");
if (File.Exists(confPath))
{
    builder.Configuration.AddJsonFile(confPath);
}
#endif


builder.Services.AddValidatorsFromAssemblyContaining<Program>();
builder.Services.AddSystemd();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();
builder.Services.AddAria2();
builder.Services.AddTelegramBot();
builder.Services.AddTransient<UpdateHandler>();

builder.Services
    .AddOptions<Aria2Settings>()
    .Bind(builder.Configuration.GetRequiredSection("Aria2"))
    .ValidateFluently()
    .ValidateOnStart();

builder.Services
    .AddOptions<TelegramSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Telegram"))
    .ValidateFluently()
    .ValidateOnStart();

builder.Services
    .AddOptions<JellyfinSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Jellyfin"))
    .ValidateFluently()
    .ValidateOnStart();

builder.Services
    .AddOptions<ApiSettings>()
    .Bind(builder.Configuration.GetRequiredSection("Api"))
    .ValidateFluently()
    .ValidateOnStart();


var app = builder.Build();

await app.RunAsync();