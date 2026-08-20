using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.ExporterService
{
    public interface IExportService
    {
        Task<ResultExport> ExportAsync(JobExecutionResult result, ExportFormat format);
    }
}
