using ChatAppApi.Repositories;
using ChatAppApi.Services;
using ChatAppApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ChatAppApi.Exceptions;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using ChatAppApi.Dtos;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add Database connection
// MySQL
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("MySqlLocal"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MySqlLocal"))
     )
);
// Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    string configuration = builder.Configuration.GetConnectionString("RedisLocal") ?? "";
    return ConnectionMultiplexer.Connect(configuration);
});

// Add JWT settings and authorization
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    Byte[] key = Encoding.UTF8.GetBytes(builder.Configuration["Jwt:SecretKey"] ?? "");
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidateAudience = false,
        ValidateLifetime = true,
        ClockSkew = TimeSpan.Zero
    };
    options.Events = new JwtBearerEvents
    {
        OnTokenValidated = async context =>
        {
            string? jti = context.Principal?.FindFirst(JwtRegisteredClaimNames.Jti)?.Value;
            if (string.IsNullOrEmpty(jti))
            {
                context.HttpContext.Items["Error"] = "JTI not found";
                context.Fail("JTI not found");
                return;
            }
            RedisService redisService = context.HttpContext.RequestServices.GetRequiredService<RedisService>();
            string? logout = await redisService.GetStringAsync(jti);
            if (logout != null)
            {
                context.HttpContext.Items["Error"] = "This token has been revoked";
                context.Fail("This token has been revoked");
            }
        },
        OnChallenge = async context =>
        {
            context.HandleResponse();

            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            context.Response.ContentType = "application/json";

            string message = context.HttpContext.Items["Error"] as string
                          ?? context.ErrorDescription
                          ?? "Unauthorized";

            var responseBody = ApiResponse<object?>.CreateFail(message);
            await context.Response.WriteAsJsonAsync(responseBody);
        }
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("ROLE_USER", policy => policy.RequireClaim("scope", "ROLE_USER"));
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
// Add Transactional
builder.Services.AddScoped<Transactional>();
// Add services to the container.
builder.Services.AddScoped<JwtUtils>();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<AuthenticationService>();

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseAuthentication();
app.UseAuthorization();

// Add middlewares
app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

app.Run();
