using System.Text;
using ApiGympass.Data;
using ApiGympass.Data.Repositories.Implementations;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Implementations;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GympassContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<User, IdentityRole<Guid>>(options =>
    {
        options.User.AllowedUserNameCharacters = "aãbcdefghijklmnoõpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
        options.User.RequireUniqueEmail = true;
    })
    .AddEntityFrameworkStores<GympassContext>()
    .AddDefaultTokenProviders();

var key = Encoding.ASCII.GetBytes(builder.Configuration["Jwt:Key"]);
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.RequireHttpsMetadata = false; // Set to false if testing locally without HTTPS
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(key),
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero,
        };
    });

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
});

builder.Services.AddAuthorization(options => 
{
    options.AddPolicy("RequireAdminRole", policy => policy.RequireRole("Admin"));
    options.AddPolicy("RequireMemberRole", policy => policy.RequireRole("Member"));
});

// AutoMapper configuration
builder.Services.
    AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICheckInRepository, CheckInRepository>();
builder.Services.AddScoped<IGymRepository, GymRepository>();
builder.Services.AddScoped<TokenService>();

// Services
builder.Services.AddScoped<IUserService, UserService>();
builder.Services.Decorate<IUserService, ArchivingUserService>();

builder.Services.AddScoped<ICheckInService, CheckInService>();
builder.Services.AddScoped<IGymService, GymService>();

builder.Services.AddControllers().AddNewtonsoftJson();

// Controllers
builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.MapControllers();

app.UseStaticFiles();

app.Run();
