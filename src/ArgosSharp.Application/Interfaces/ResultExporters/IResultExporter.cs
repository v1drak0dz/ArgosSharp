using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;

namespace ArgosSharp.Application.Interfaces.ResultExporters
{
    public interface IResultExporter
    {
        bool CanHandle(ExportFormat format);

        Task<ResultExport> Export(JobExecutionResult result);
    }
}
