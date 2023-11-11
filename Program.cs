using ApiGympass.Data;
using ApiGympass.Data.Repositories.Implementations;
using ApiGympass.Data.Repositories.Interfaces;
using ApiGympass.Models;
using ApiGympass.Services.Implementations;
using ApiGympass.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<GympassContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services
    .AddIdentity<User, IdentityRole<Guid>>(options =>
    {
      options.User.AllowedUserNameCharacters = "aãbcdefghijklmnoõpqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+ ";
    })
    .AddEntityFrameworkStores<GympassContext>()
    .AddDefaultTokenProviders();

// AutoMapper configuration
builder.Services.
    AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

// Services and Repositories for DI

// Repository
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<ICheckInRepository, CheckInRepository>();
builder.Services.AddScoped<IGymRepository, GymRepository>();

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

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
