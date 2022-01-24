using Refit;
using WebApp.Core.ENSEK.Models;

namespace WebApp.Core.ENSEK.UseCases;

public interface IAccount
{
    [Get("/Account/upload-meter-readings")]
    Task<UploadResponse> UploadMeterReadings([Body] UploadRequest request);
}