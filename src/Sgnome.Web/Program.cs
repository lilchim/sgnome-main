using Sgnome.Models.Graph;
using SteamApi.Client.Extensions;
using PlayerService;
using PlayerService.Providers;
using LibraryService;
using LibraryService.Database;
using Sgnome.Clients.Steam;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Vite dev server
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add Redis
builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    var redisConnectionString = configuration.GetConnectionString("Redis") ?? "localhost:6379";
    return ConnectionMultiplexer.Connect(redisConnectionString);
});

// Add Steam API client
builder.Services.AddSteamApiClient(options =>
{
    options.BaseUrl = builder.Configuration["SteamApi:BaseUrl"] ?? "http://localhost:8080";
    options.ApiKey = builder.Configuration["SteamApi:ApiKey"] ?? "dev-key";
});

// Add Steam client with caching
builder.Services.AddSteamClient();

// Add Player services
builder.Services.AddScoped<ISteamPlayerProvider, SteamPlayerProvider>();
builder.Services.AddScoped<PlayerService.PlayerAggregator>();
builder.Services.AddScoped<PlayerService.Database.IPlayerDatabase, PlayerService.Database.RedisPlayerDatabase>();
builder.Services.AddScoped<IPlayerService, PlayerService.PlayerService>();

// Add Library services
builder.Services.AddLibraryService();

var app = builder.Build();

// Configure the HTTP request pipeline.
// Enable Swagger for development and when explicitly requested
if (app.Environment.IsDevelopment() || app.Configuration["EnableSwagger"] == "true")
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowFrontend");

// Serve static files (built Svelte app)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseAuthorization();
app.MapControllers();

// Fallback to Svelte app for client-side routing
app.MapFallbackToFile("index.html");

app.Run(); 