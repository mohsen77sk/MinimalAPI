namespace Minimal.Api.Common.DeviceDetectionService;

public interface IDeviceDetectionService
{
    string GetDeviceDetails(HttpContext context);
    string GetCurrentRequestDeviceDetails();

    string GetDeviceDetailsHash(HttpContext context);
    string GetCurrentRequestDeviceDetailsHash();
}