using ArgosSharp.Application.Interfaces.Repositories;
using ArgosSharp.Domain.Entity;
using ArgosSharp.Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace ArgosSharp.Infrastructure.Repositories
{
    public class JobRepository(ArgosDbContext argosDbContext) : IJobRepository
    {
        public async Task AddAsync(Job job)
        {
            argosDbContext.Jobs.Add(job);
            await argosDbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(Job job)
        {
            argosDbContext.Update(job);
            await argosDbContext.SaveChangesAsync();
        }

        public async Task<Job?> GetAsync(int Id) =>
            await argosDbContext.Jobs.FirstOrDefaultAsync(x => x.Id == Id);

        public async Task<List<Job>> GetAllAsync() =>
            await argosDbContext.Jobs.ToListAsync();

    }
}
