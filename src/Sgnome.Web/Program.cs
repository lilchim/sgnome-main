using Sgnome.Models.Graph;
using SteamApi.Client.Extensions;
using PlayerService;
using PlayerService.Providers;
using LibraryService;
using LibraryService.Providers;

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

// Add Steam API client
builder.Services.AddSteamApiClient(options =>
{
    options.BaseUrl = builder.Configuration["SteamApi:BaseUrl"] ?? "http://localhost:8080";
    options.ApiKey = builder.Configuration["SteamApi:ApiKey"] ?? "dev-key";
});

// Add Player services
builder.Services.AddScoped<ISteamPlayerProvider, SteamPlayerProvider>();
builder.Services.AddScoped<PlayerService.PlayerAggregator>();
builder.Services.AddScoped<IPlayerService, PlayerService.PlayerService>();

// Add Library services
builder.Services.AddScoped<ISteamLibraryProvider, SteamLibraryProvider>();
builder.Services.AddScoped<LibraryService.LibraryAggregator>();
builder.Services.AddScoped<ILibraryService, LibraryService.LibraryService>();

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