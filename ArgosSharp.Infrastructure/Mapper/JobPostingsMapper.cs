using ArgosSharp.Domain.ValueObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArgosSharp.Infrastructure.Mapper
{
    public static class JobPostingMapper
    {
        public static JobPosting Map(
            string title,
            string link,
            string description,
            string source)
        {
            return new JobPosting
            {
                Title = title,
                Link = link,
                Description = description,
                Source = source
            };
        }
    }
}
