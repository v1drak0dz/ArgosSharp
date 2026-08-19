using ArgosSharp.Application.Interfaces.ResultExporters;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using CsvHelper;
using System.Globalization;
using System.Text;

namespace ArgosSharp.Infrastructure.ResultExporters
{
    internal class CSVResultExporter : IResultExporter
    {
        public bool CanHandle(ExportFormat format) => format == ExportFormat.CSV;

        public async Task<ResultExport> ExportAsync(JobExecutionResult result)
        {
            using var writer = new StringWriter();

            using var csv = new CsvWriter(writer, CultureInfo.InvariantCulture);

            csv.WriteRecords(result.Data);

            var bytes = Encoding.UTF8.GetBytes(writer.ToString());

            var stream = new MemoryStream(bytes);

            return new ResultExport { Content = stream, FileName = "export.csv", ContentType = "csv", Extension = ".csv" };
        }
    }
}
