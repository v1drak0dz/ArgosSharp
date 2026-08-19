using ArgosSharp.Application.Interfaces.ResultExporters;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Services.ExporterService
{
    internal class ExportService(IEnumerable<IResultExporter> exporters) : IExportService
    {
        public Task<ResultExport> ExportAsync(JobExecutionResult result, ExportFormat format) =>
            exporters
                .FirstOrDefault(e => e.CanHandle(format))?
                .ExportAsync(result) 
            ?? throw new NotSupportedException($"Export format {format} is not supported.");
    }
}
