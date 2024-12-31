using Common.Infrastructure.EntityFramework.Extensions;
using Common.Infrastructure.Neo4J.Extensions;
using Common.Infrastructure.RabbitMq.Extensions;
using Core.Application.Extensions;
using Host.QueueListener;
using Host.QueueListener.ProjectionWriters;

var builder = Microsoft.Extensions.Hosting.Host.CreateApplicationBuilder(args);
builder.Services.AddHostedService<QueueListener>();

var pgConnectionString = builder.Configuration.GetConnectionString("Postgres");
if (string.IsNullOrEmpty(pgConnectionString))
{
    throw new InvalidOperationException("No pq connection string found.");
}

var rabbitMqConnectionString = builder.Configuration.GetConnectionString("RabbitMq");
if (string.IsNullOrEmpty(rabbitMqConnectionString))
{
    throw new InvalidOperationException("No rabbitmq connection string found.");
}

var neo4JUrl = builder.Configuration.GetRequiredSection("Neo4J:Url").Value;
if (string.IsNullOrEmpty(neo4JUrl))
{
    throw new InvalidOperationException("No neo4j url found.");
}

var neo4JUsername = builder.Configuration.GetRequiredSection("Neo4J:Username").Value;
if (string.IsNullOrEmpty(neo4JUsername))
{
    throw new InvalidOperationException("No neo4j username found.");
}

var neo4JPassword = builder.Configuration.GetRequiredSection("Neo4J:Password").Value;
if (string.IsNullOrEmpty(neo4JPassword))
{
    throw new InvalidOperationException("No neo4j password found.");
}

await builder.Services
    .AddCoreApplication()
    .AddCommonInfrastructureEntityFramework(pgConnectionString)
    .AddCommonInfrastructureNeo4J(neo4JUrl, neo4JUsername, neo4JPassword)
    .AddCommonInfrastructureRabbitMq(rabbitMqConnectionString);

builder.Services.AddTransient<CommentProjectionWriter>();

var host = builder.Build();
host.Run();