using ChatAppApi.Dtos;
using ChatAppApi.Exceptions;
using ChatAppApi.Hubs;
using ChatAppApi.Repositories;
using ChatAppApi.Requirements;
using ChatAppApi.Services;
using ChatAppApi.Utils;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using StackExchange.Redis;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

// Load .env file
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

// Add Requirements (Filter)
builder.Services.AddHttpContextAccessor();
builder.Services.AddSingleton<IAuthorizationHandler, AdminOrOwnerHandler>();

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
        // trước khi validate token (dùng để lấy token từ 1 nơi nào đó, áp dụng cho toàn bộ request)
        OnMessageReceived = context =>
        {
            string? accessToken = context.Request.Query["access_token"];
            PathString path = context.HttpContext.Request.Path;
            // chỉ áp dụng cho websocket, lấy token từ query string (mặc định là từ header)
            if (!string.IsNullOrEmpty(accessToken) && path.StartsWithSegments("/applicationHub"))
            {
                context.Token = accessToken;
            }
            return Task.CompletedTask;  // nếu không có [Authorize] thì không thực hiện validate
        },
        // sau khi token validate thành công
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
        // sau khi token validate thành công nhưng không có quyền truy cập vào tài nguyên
        OnForbidden = async context =>
        {
            context.Response.StatusCode = StatusCodes.Status403Forbidden;
            context.Response.ContentType = "application/json";

            var responseBody = ApiResponse<object?>.CreateFail("You do not have access to this resource");
            await context.Response.WriteAsJsonAsync(responseBody);
        },
        // sau khi token validate thất bại
        OnAuthenticationFailed = context =>
        {
            context.NoResult();
            ILogger? logger = context.HttpContext.RequestServices.GetService<ILogger<JwtBearerEvents>>();
            logger?.LogWarning(context.Exception, "JWT authentication failed");
            context.HttpContext.Items["Error"] = "Invalid token";
            return Task.CompletedTask;
        },
        // sau OnAuthenticationFailed
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
    options.AddPolicy("ROLE_USER", policy => policy.RequireClaim("scope", "ROLE_USER", "ROLE_ADMIN"));
    options.AddPolicy("ROLE_ADMIN", policy => policy.RequireClaim("scope", "ROLE_ADMIN"));
    options.AddPolicy("ADMIN_OR_OWNER", policy => policy.Requirements.Add(new AdminOrOwnerRequirement()));
});

// Add AutoMapper
builder.Services.AddAutoMapper(typeof(Program));
// Add Transactional
builder.Services.AddScoped<Transactional>();
// Add SignalR
builder.Services.AddSignalR();
// Add services to the container.
builder.Services.AddScoped<JwtUtils>();
builder.Services.AddScoped<RedisService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<UserRepository>();
builder.Services.AddScoped<RoleRepository>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<FriendshipService>();
builder.Services.AddScoped<FriendshipRepository>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddScoped<NotificationRepository>();
builder.Services.AddScoped<ConversationRepository>();
builder.Services.AddScoped<ConversationService>();

// Cors
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowSpecificOrigin", policy =>
    {
        policy.WithOrigins("http://localhost:5173", "https://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

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

app.UseCors("AllowSpecificOrigin");
app.UseAuthentication();
app.UseAuthorization();

// Add middlewares
app.UseMiddleware<GlobalExceptionHandler>();

app.MapControllers();

// Add Hub (Websocket - SignalR)
app.MapHub<ApplicationHub>("/applicationHub");

app.Run();
