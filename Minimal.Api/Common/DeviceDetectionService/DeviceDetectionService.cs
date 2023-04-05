using Microsoft.Net.Http.Headers;
using Minimal.Api.Common.IdentityServices;
using UAParser;

namespace Minimal.Api.Common.DeviceDetectionService;

public class DeviceDetectionService : IDeviceDetectionService
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ISecurityService _securityService;

    public DeviceDetectionService(ISecurityService securityService, IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor ?? throw new ArgumentNullException(nameof(httpContextAccessor));
        _securityService = securityService ?? throw new ArgumentNullException(nameof(securityService));
    }

    public string GetDeviceDetails(HttpContext? context)
    {
        var ua = GetUserAgent(context);
        if (ua is null)
        {
            return "unknown";
        }

        var client = Parser.GetDefault().Parse(ua);
        var deviceInfo = client.Device.Family;
        var browserInfo = $"{client.UA.Family}, {client.UA.Major}.{client.UA.Minor}.{client.UA.Patch}";
        var osInfo = $"{client.OS.Family}, {client.OS.Major}.{client.OS.Minor}.{client.OS.Patch}";
        var ipAddress = GetClientIpAddress(context);
        return $"{deviceInfo}, {browserInfo}, {osInfo} - {ipAddress}";
    }

    public string GetCurrentRequestDeviceDetails() => GetDeviceDetails(_httpContextAccessor.HttpContext);

    public string GetDeviceDetailsHash(HttpContext? context) => _securityService.GetSha256Hash(GetDeviceDetails(context));

    public string GetCurrentRequestDeviceDetailsHash() => GetDeviceDetailsHash(_httpContextAccessor.HttpContext);


    private static string? GetUserAgent(HttpContext? context)
    {
        if (context is null)
        {
            return null;
        }

        return context.Request.Headers.TryGetValue(HeaderNames.UserAgent, out var userAgent)
            ? userAgent.ToString()
            : null;
    }

    private static string? GetClientIpAddress(HttpContext? context)
    {
        if (context is null)
        {
            return null;
        }

        var ipAddress = context.Connection.RemoteIpAddress?.ToString();

        if (string.IsNullOrEmpty(ipAddress))
        {
            ipAddress = context?.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        }

        return ipAddress;
    }
}