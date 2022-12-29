using Minimal.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
// Configure logging
builder.Logging.ClearProviders();
if (builder.Environment.IsDevelopment())
{
    builder.Logging.AddConsole();
    builder.Logging.AddDebug();
}
builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));
// Configure services
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();
app.ConfigureApplication();
app.ConfigureDatabase();

app.Run();
