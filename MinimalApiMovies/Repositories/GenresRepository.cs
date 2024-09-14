using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.Entities;
using System.Linq.Expressions;

namespace MinimalApiMovies.Repositories {
    public class GenresRepository : IGenresRepository {
        ApplicationDbContext context;
        public GenresRepository(ApplicationDbContext context) {
            this.context = context;
        }
        public async Task<int> Create(Genre genre) {
            context.Add(genre);
            await context.SaveChangesAsync();
            return genre.Id;
        }

        public async Task<bool> Exists(int id) {
            return await context.Genres.AnyAsync(x => x.Id == id);
        }

        public async Task<List<int>> Exists(List<int> ids) {
            return await context.Genres
                .Where(g => ids.Contains(g.Id)).Select(g => g.Id)
                .ToListAsync();
            ;
        }

        public async Task<List<Genre>> GetAll() {
            return await context.Genres.OrderBy(g => g.Name).ToListAsync();
        }

        public async Task<Genre?> GetById(int id) {
            return await context.Genres.FirstOrDefaultAsync(g => g.Id == id);
        }

        public async Task Update(Genre genre) {
            context.Update(genre);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id) {
            var genre = await GetById(id);
            if( genre != null ) {
                context.Genres.Remove(genre);
                await context.SaveChangesAsync();
            }
        }

    }
}
