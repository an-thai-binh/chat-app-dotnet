using ChatAppApi.Repositories;
using ChatAppApi.Services;
using ChatAppApi.Utils;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ChatAppApi.Exceptions;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

// Add Database connection
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("Local"),
        ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("Local"))
     )
);
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
