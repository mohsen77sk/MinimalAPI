using Minimal.Api.Extensions;

var builder = WebApplication.CreateBuilder(args);
builder.AddApplicationLogging();
builder.Services.AddApplicationServices(builder.Configuration);

var app = builder.Build();
app.ConfigureApplication();
app.ConfigureDatabase();

app.Run();
