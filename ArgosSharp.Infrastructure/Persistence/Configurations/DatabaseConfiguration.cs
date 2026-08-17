using Microsoft.Extensions.Configuration;

namespace ArgosSharp.Infrastructure.Persistence.Configurations
{
    internal static class DatabaseConfiguration
    {
        internal static string BuildConnectionString(IConfiguration conf)
        {
            var host = conf["DB_HOST"];
            var port = conf["DB_PORT"];
            var db = conf["DB_NAME"];
            var user = conf["DB_USER"];
            var passwd = conf["DB_PASSWORD"];

            return
                $"Host={host};" +
                $"Port={port};" +
                $"Database={db};" +
                $"Username={user};" +
                $"Password={passwd};";
        }
    }
}
