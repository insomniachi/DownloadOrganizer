using Aria2NET;
using Aria2TelegramBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

var builder = Host.CreateApplicationBuilder(args);
#if DEBUG
builder.Configuration.AddEnvironmentVariables();
#else
var confPath = builder.Configuration.GetValue<string>("ConfigPath");
if (File.Exists(confPath))
{
    builder.Configuration.AddJsonFile(confPath);
}
#endif


builder.Services.AddSystemd();
builder.Services.AddHttpClient();
builder.Services.AddHostedService<Worker>();
builder.Services.AddAria2();
builder.Services.AddTelegramBot();
builder.Services.AddTransient<UpdateHandler>();

var app = builder.Build();

await app.RunAsync();
