using System.Text;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.JsonWebTokens;
using Microsoft.IdentityModel.Tokens;
using Presentation.Hubs;
using Presentation.Logging;
using StudyHub.Core.Constants;
using StudyHub.Infrastructure.Config;
using StudyHub.Infrastructure.Data;

var builder = WebApplication.CreateBuilder(args);
// Configure Logger
builder.ConfigureSerilog();

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddSignalR()
    .AddStackExchangeRedis(builder.Configuration.GetConnectionString("Redis"));

// Infrastructure
builder.Services.AddInfrastructure(builder.Configuration);

//testing
builder.Logging.AddFilter("Microsoft.AspNetCore.Authentication", LogLevel.Debug);
builder.Logging.AddFilter("Microsoft.AspNetCore.Authorization", LogLevel.Debug);

var jwtKey = builder.Configuration["Jwt:Key"];
var jwtIssuer = builder.Configuration["Jwt:Issuer"];
var jwtAudience = builder.Configuration["Jwt:Audience"];
//end testing


Console.WriteLine($"JWT Key: {jwtKey}");
Console.WriteLine($"JWT Issuer: {jwtIssuer}");
Console.WriteLine($"JWT Audience: {jwtAudience}");

// Healthchecks
builder.Services.AddHealthChecks()
    .AddNpgSql(builder.Configuration.GetConnectionString("DefaultConnection")!)
    .AddRedis(builder.Configuration.GetConnectionString("Redis")!);

// JWT
builder.Services.AddAuthentication(o =>
    {
        o.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        o.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
    })
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!))
        };

        //ALLOW jwt auth over Signalr websockets  
        options.Events = new JwtBearerEvents
        {
            OnMessageReceived = context =>
            {
                var accessToken = context.Request.Query["access_token"];
                var path = context.HttpContext.Request.Path;
                if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/chat"))
                    context.Token = accessToken;
                
                // Debug: Log the actual token being received
                var authHeader = context.Request.Headers["Authorization"].FirstOrDefault();
                Console.WriteLine($"Auth Header: '{authHeader}'");
            
                if (authHeader?.StartsWith("Bearer ") == true)
                {
                    var token = authHeader.Substring("Bearer ".Length);
                    Console.WriteLine($"Extracted Token: '{token}'");
                    Console.WriteLine($"Token Length: {token.Length}");
                    Console.WriteLine($"Token has dots: {token.Contains('.')}");
                }
                //end debug
                
                return Task.CompletedTask;
            },
            
            //testing
            OnAuthenticationFailed = context =>
            {
                Console.WriteLine($"JWT Authentication failed: {context.Exception.Message}");
                Console.WriteLine($"Token being validated: '{context.Request.Headers["Authorization"]}'");
                return Task.CompletedTask;
            },
            OnTokenValidated = context =>
            {
                Console.WriteLine("JWT Token validated successfully");
                return Task.CompletedTask;
            }
            //end testing
        };

    });


// builder.Services.AddAuthorization(options =>
// {
//     options.AddPolicy("RequireAdmin", p=> p.RequireRole(Roles.Admin));
// });

var allowedOrigins = builder.Configuration.GetSection("AllowedOrigins").Get<string[]>();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy => policy
            .WithOrigins(allowedOrigins ?? Array.Empty<string>())
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials());
});

builder.Services.AddAuthorization();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors("AllowFrontend");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHub<ChatHub>("/chat");
app.MapHealthChecks("/health", new()
{
    ResponseWriter = async (ctx, report) =>
    {
        ctx.Response.ContentType = "application/json";
        await UIResponseWriter.WriteHealthCheckUIResponse(ctx, report);
    }
});

// --- Seed (admin user + sample room) ---
await SeedData.EnsureSeededAsync(app.Services, app.Configuration);

app.Run();