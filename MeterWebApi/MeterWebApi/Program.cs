using Data;
using Data.Implementation;
using Data.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle

builder.Services.AddSwaggerGen();
builder.Services.AddControllers();

// Configure database
builder.Services.AddDb(builder.Configuration.GetConnectionString("Default"));

// Register DI middleware
builder.Services.AddScoped<IRepository, Repository>();
builder.Services.AddScoped<IReadingInterface, ReadingInterface>();

var app = builder.Build();

// Ensure database is created on startup (dev convenience) and seed accounts
await app.Services.EnsureCreatedAndSeedAsync();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();

await app.RunAsync();
