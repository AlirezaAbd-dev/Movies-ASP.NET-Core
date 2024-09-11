using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Repositories {
    public class ActorsRepository(ApplicationDbContext context) : IActorsRepository
    {
        public async Task<List<Actor>> GetAll()
        {
            return await context.Actors.OrderBy(a => a.Name).ToListAsync();
        }

        public async Task<Actor?> GetById(int id)
        {
            return await context.Actors.AsNoTracking().FirstOrDefaultAsync(a=> a.Id == id);
        }

        public async Task<int> Create(Actor actor)
        {
            context.Add(actor);
            await context.SaveChangesAsync();
            return actor.Id;
        }

        public async Task<bool> Exists(int id)
        {
            return await context.Actors.AnyAsync(a => a.Id == id);
        }

        public async Task Update(Actor actor) {
            context.Update(actor);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id)
        {
            await context.Actors.Where(a => a.Id == id).ExecuteDeleteAsync();
        }
    }
}
