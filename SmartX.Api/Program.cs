var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// ============================================================
// EXISTING PART 1 SERVICES
// ============================================================

builder.Services.AddSingleton<SmartX.Api.Services.TelemetryService>();
builder.Services.AddSingleton<SmartX.Api.Services.DeploymentValidator>();
builder.Services.AddSingleton<SmartX.Api.Services.TelemetryProcessor>();
builder.Services.AddSingleton<SmartX.Api.Services.RealTimeProcessingService>();
// ============================================================
// PART 2 SERVICES
// ============================================================

// Stores manual device commands using Stack<T>.
// Singleton keeps command history available throughout
// the lifetime of the application.
builder.Services.AddSingleton<SmartX.Api.Services.CommandHistoryService>();
builder.Services.AddSingleton<SmartX.Api.Services.RecommendationService>();
builder.Services.AddSingleton<SmartX.Api.Services.SearchHistoryService>();
// ============================================================
// CORS
// ============================================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

// ============================================================
// SWAGGER
// ============================================================

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// ============================================================
// MIDDLEWARE
// ============================================================

app.UseHttpsRedirection();

app.UseCors("AllowFrontend");

app.UseAuthorization();

app.MapControllers();

app.Run();