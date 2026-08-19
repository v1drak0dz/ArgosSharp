using ArgosSharp.Application.Interfaces.ResultExporters;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using System.Text;
using System.Text.Json;

namespace ArgosSharp.Infrastructure.ResultExporters
{
    internal class JsonResultExporter : IResultExporter
    {
        public bool CanHandle(ExportFormat format) => format == ExportFormat.JSON;
        public Task<ResultExport> Export(JobExecutionResult result)
        {
            var json = JsonSerializer.Serialize(result.Data);
            var bytes = Encoding.UTF8.GetBytes(json);
            var stream = new MemoryStream(bytes);
            return Task.FromResult(new ResultExport
            { 
                Content = stream,
                FileName = "export.json",
                ContentType = "application/json",
                Extension = ".json"
            });
        }
    }
}
