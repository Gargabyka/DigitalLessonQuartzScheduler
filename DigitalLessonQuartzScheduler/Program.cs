using DigitalLessonQuartzScheduler;
using DigitalLessonQuartzScheduler.Factory;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
var configBuilder = new ConfigurationBuilder().SetBasePath(Environment.CurrentDirectory)
    .AddJsonFile("appsettings.json");

var config = configBuilder.Build();

builder.Host.UseSerilog((context, logConfig) => logConfig
    .ReadFrom.Configuration(context.Configuration)        
    .WriteTo.Console());

builder.Services.AddGrpc();
builder.Services.AddGrpcHealthChecks();

builder.Services.AddGrpcClients(config);
builder.Services.QuartzRegister(config);

var app = builder.Build();

var quartz = app.Services.GetService<IQuartzFactory>();
var scheduler = quartz?.GetScheduler();

scheduler?.Start().Wait();

app.UseRouting();

app.Run();