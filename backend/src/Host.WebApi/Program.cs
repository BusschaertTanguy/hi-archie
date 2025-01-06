using System.Net;
using System.Text.Json.Serialization;
using Common.Infrastructure.EntityFramework.Extensions;
using Common.Infrastructure.Neo4J.Extensions;
using Common.Infrastructure.RabbitMq.Extensions;
using Core.Application.Extensions;
using Host.WebApi.Handlers;
using Host.WebApi.Routes;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

// TODO: Options pattern

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

var clientUrl = builder.Configuration.GetRequiredSection("Client:Url").Value;
if (string.IsNullOrEmpty(clientUrl))
{
    throw new InvalidOperationException("No client url found.");
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

builder.Services
    .AddCoreApplication()
    .AddCommonInfrastructureEntityFramework(pgConnectionString)
    .AddCommonInfrastructureNeo4J(neo4JUrl, neo4JUsername, neo4JPassword)
    .AddCommonInfrastructureRabbitMq(rabbitMqConnectionString);

builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.Configure<JsonOptions>(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddCors(options => options.AddPolicy("ClientPolicy",
    policyBuilder => policyBuilder.WithOrigins(clientUrl).AllowAnyHeader().AllowAnyMethod()));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen(options =>
    {
        options.CustomSchemaIds(type => type.FullName?
            .Replace("+", ".")
            .Replace("Core.Application.", string.Empty)
            .Replace("Core.Domain.", string.Empty)
            .Replace("Microsoft.AspNetCore.Mvc.", string.Empty) ?? string.Empty);

        options.UseAllOfToExtendReferenceSchemas();
        options.SupportNonNullableReferenceTypes();
        options.NonNullableReferenceTypesAsRequired();
    });
}

builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.Authority = builder.Configuration.GetRequiredSection("OAuth:Authority").Value;
        options.Audience = builder.Configuration.GetRequiredSection("OAuth:Audience").Value;
    });

builder.Services.AddAuthorization();

builder.Services.AddTransient<IAuthorizationHandler, UserIsCommunityOwnerAuthorizationHandler>();
builder.Services.AddTransient<IAuthorizationHandler, UserIsPostOwnerAuthorizationHandler>();
builder.Services.AddTransient<IAuthorizationHandler, UserIsCommentOwnerAuthorizationHandler>();

var app = builder.Build();

app.UseExceptionHandler();

if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseHttpsRedirection();
}

app.UseCors("ClientPolicy");
app.UseAuthentication();
app.UseAuthorization();

var api = app.MapGroup("api");
api.ProducesProblem((int)HttpStatusCode.InternalServerError);

var v1 = api.MapGroup("v1");

v1.MapUserRoutes();
v1.MapCommunityRoutes();
v1.MapPostRoutes();
v1.MapCommentRoutes();

app.Run();