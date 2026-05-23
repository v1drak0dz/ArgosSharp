using ArgosSharp.Application.Interfaces.Strategies;
using ArgosSharp.Domain.Annotations;
using ArgosSharp.Domain.Enums;
using ArgosSharp.Domain.ValueObjects;
using System.Reflection;

namespace ArgosSharp.Application.StrategiesContext.Scraper
{
    public class ScraperStrategyContext : IScraperStrategyContext
    {
        public async Task<List<Noticia>> GetNoticiasBySourceAsync(ScraperSourceEnum scraperSource, string searchTerm, int depth)
        {
            var scrapers = Assembly.GetExecutingAssembly().GetTypes().Where(x => typeof(IScraperStrategy).IsAssignableFrom(x) && !x.IsInterface && !x.IsAbstract);
            
            foreach(var scraper in scrapers)
            {
                var attr = scraper.GetCustomAttribute<ScraperSourceAnnotation>();

                if (attr is null)
                    continue;

                if (attr.ScraperSource == scraperSource)
                {
                    var instance = Activator.CreateInstance(scraper) as IScraperStrategy;

                    if (instance is null)
                        continue;

                    return await instance.ProcessScraperAsync(searchTerm, depth);
                }
            }

            throw new Exception($"No Scraper found for source {scraperSource}");
        }
    }
}
