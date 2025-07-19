namespace Minimal.Api.Extensions;

public static class LoggingExtensions
{
    public static WebApplicationBuilder AddApplicationLogging(this WebApplicationBuilder builder)
    {
        if (builder == null)
        {
            throw new ArgumentNullException(nameof(builder));
        }

        builder.Logging.ClearProviders();

        if (builder.Environment.IsDevelopment())
        {
            builder.Logging.AddDebug();
            builder.Logging.AddConsole();
        }

        builder.Logging.AddConfiguration(builder.Configuration.GetSection("Logging"));

        return builder;
    }
}