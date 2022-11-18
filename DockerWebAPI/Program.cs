using System.Net;
using Amazon;
using Amazon.Runtime;
using Amazon.S3;
using Amazon.SQS;
using DockerWebAPI;
using DockerWebAPI.Events;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Newtonsoft.Json;
using Rebus.Auditing.Messages;
using Rebus.Config;
using Rebus.Retry.FailFast;
using Rebus.Retry.Simple;
using Rebus.Routing.TypeBased;
using Serilog;
using Serilog.Context;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var sqlConnectionstring = "Server=localhost;Database=Rebus-test;Uid=root;Pwd=root;";
builder.Services.AddHealthChecks()
    .AddMySql(
        sqlConnectionstring);

ConfigureLogging(builder);
ConfigureRebus(builder, sqlConnectionstring);

var app = builder.Build();

using (LogContext.PushProperty("Scope", "App Starting"))
{
    Log.Information("Starting");
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseAuthorization();

    app.MapControllers();
    app.UseRouting().UseEndpoints(x => { });

    Log.Information("Finished");
    app.Run();
}

void ConfigureRebus(WebApplicationBuilder webApplicationBuilder, string s)
{
    var d = JsonConvert.DeserializeObject<Configs>(File.ReadAllText("credentials.json"));
    webApplicationBuilder.Services.AutoRegisterHandlersFromAssemblyOf<Step1Handler>();
    webApplicationBuilder.Services.AddRebus(
        configure => configure
            .Logging(x => x.Serilog())
            .Subscriptions(x => x.StoreInMySql(s, "rebus_subscribe_table"))
            .Transport(t =>
            {
                var amazonSqsConfig = new AmazonSQSConfig
                {
                    
                };
                t.UseAmazonSQS(new BasicAWSCredentials(d.AccessKey,d.SecretKey), amazonSqsConfig, "https://sqs.eu-central-1.amazonaws.com/078884735169/rebustest");
            })
            // .Transport(t => t.UseMySql(s, "rebus_message_table", "rebus_queue_something"))
            .Options(x=>
            {
                // x.EnableMessageAuditing("audit_channel");
                x.SimpleRetryStrategy();
                x.SetMaxParallelism(2);
                x.SetNumberOfWorkers(2);
                // x.SetNumberOfWorkers();
            })
            .Routing(x => x.TypeBased())
    );
}

void ConfigureLogging(WebApplicationBuilder builder1)
{
    Log.Logger = new LoggerConfiguration()
        .WriteTo.Seq("http://localhost:5341")
        .Enrich.WithProperty("appname", "rebus-test")
        .Enrich.FromLogContext()
        .CreateLogger();

    builder1.Logging.AddSerilog();
}