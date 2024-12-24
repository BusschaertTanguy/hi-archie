using System.Net;
using System.Text.Json.Serialization;
using Common.Infrastructure.Data.Extensions;
using Core.Application.Extensions;
using Host.WebApi.Handlers;
using Host.WebApi.Routes;
using Host.WebApi.Transformers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddProblemDetails();

var connectionString = builder.Configuration.GetConnectionString("Postgres");

if (string.IsNullOrEmpty(connectionString))
{
    throw new InvalidOperationException("No connection string found.");
}

var clientUrl = builder.Configuration.GetRequiredSection("Client:Url").Value;

if (string.IsNullOrEmpty(clientUrl))
{
    throw new InvalidOperationException("No client url found.");
}

builder.Services
    .AddCommonInfrastructureData(connectionString)
    .AddCoreApplication();

builder.Services.ConfigureHttpJsonOptions(options => { options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()); });
builder.Services.Configure<JsonOptions>(options => { options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter()); });

builder.Services.AddCors(options => options.AddPolicy("ClientPolicy",
    policyBuilder => policyBuilder.WithOrigins(clientUrl).AllowAnyHeader().AllowAnyMethod()));

if (builder.Environment.IsDevelopment())
{
    builder.Services.AddOpenApi(options =>
    {
        options.CreateSchemaReferenceId = info =>
        {
            string[] assemblyNames = ["Core.Application.", "Core.Domain."];
            var fullName = info.Type.FullName?.Replace("+", ".") ?? string.Empty;

            return assemblyNames
                .Where(assemblyName => fullName.StartsWith(assemblyName))
                .Select(assemblyName => fullName.Replace(assemblyName, string.Empty))
                .FirstOrDefault();
        };

        options.AddSchemaTransformer<OptionalPropertySchemaTransformer>();
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
    app.MapOpenApi();
    app.UseSwaggerUI(options => { options.SwaggerEndpoint("/openapi/v1.json", "v1"); });
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