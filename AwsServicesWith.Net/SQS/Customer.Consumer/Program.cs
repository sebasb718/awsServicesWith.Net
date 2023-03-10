using Amazon.SQS;
using Customer.Consumer;
using Customers.Consumer;

var builder = WebApplication.CreateBuilder(args);

builder.Services.Configure<QueueSettings>(builder.Configuration.GetSection(QueueSettings.Key));
builder.Services.AddSingleton<IAmazonSQS, AmazonSQSClient>();
builder.Services.AddHostedService<QueueConsumerServices>();
builder.Services.AddMediatR(cfg => {cfg.RegisterServicesFromAssembly(typeof(Program).Assembly);});

var app = builder.Build();

app.Run();
