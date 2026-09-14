using Lumora.Api.Handlers;
using Lumora.Application;
using Lumora.Application.Configuration;
using Lumora.Infrastructure;
using Lumora.Infrastructure.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Paste the accessToken value from POST /api/Auth/signin (not the refreshToken)."
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

builder.Services.AddHttpContextAccessor();


builder.Services.AddOptions<AppSettingsConfiguration>()
    .Configure<IConfiguration>((settings, configuration) =>
    {
        configuration.Bind(settings);
    })
    .ValidateOnStart()
    .ValidateDataAnnotations();

builder.Services.AddExceptionHandler<ValidationExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Host.AddApplicationServices(builder.Configuration);
builder.Services.AddInfrastructureServices(builder.Configuration);

// Configure JWT Authentication
var jwtSettings = builder.Configuration.GetSection("Security:Jwt");
var secretKey = Encoding.ASCII.GetBytes(jwtSettings["SecretKey"]!);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.MapInboundClaims = false;
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(secretKey),
        ValidateIssuer = true,
        ValidIssuer = "lumora",
        ValidateAudience = true,
        ValidAudience = "lumora-api",
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };

    if (builder.Environment.IsDevelopment())
    {
        options.Events = new JwtBearerEvents
        {
            OnAuthenticationFailed = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtAuthentication");
                logger.LogWarning(context.Exception, "JWT authentication failed.");
                return Task.CompletedTask;
            },
            OnChallenge = context =>
            {
                var logger = context.HttpContext.RequestServices
                    .GetRequiredService<ILoggerFactory>()
                    .CreateLogger("JwtAuthentication");
                logger.LogWarning("JWT challenge issued. Error: {Error}, Description: {Description}",
                    context.Error, context.ErrorDescription);
                return Task.CompletedTask;
            }
        };
    }
});

builder.Services.AddAuthorization();

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontEnd", policy =>
    {
        policy.WithOrigins("https://localhost:4200", "http://localhost:4200")
        .AllowAnyHeader()
        .AllowAnyMethod()
        .AllowCredentials();
    });
});

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontEnd");

app.UseAuthentication();  // MUST come before UseAuthorization()
app.UseAuthorization();

app.UseExceptionHandler();
app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    await db.Database.MigrateAsync();
}

app.Run();