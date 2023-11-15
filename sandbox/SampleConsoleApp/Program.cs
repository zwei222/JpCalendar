using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using SampleConsoleApp;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.AddTransient<ConsoleApp>();

var host = builder.Build();
var app = host.Services.GetRequiredService<ConsoleApp>();

app.ShowJapaneseCalendar();
