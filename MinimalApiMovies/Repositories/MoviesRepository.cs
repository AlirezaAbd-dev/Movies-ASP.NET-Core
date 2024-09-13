using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Repositories {
    public class MoviesRepository(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context) : IMoviesRepository {
        public async Task<List<Movie>> GetAllMovies(PaginationDTO pagination) {
            var queryable = context.Movies.AsQueryable();
            await httpContextAccessor.HttpContext!
                .InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.Paginate(pagination).OrderBy(m => m.Title).ToListAsync();
        }

        public async Task<Movie?> GetById(int id) {
            return await context.Movies.AsNoTracking().FirstOrDefaultAsync(m => m.id == id);
        }

        public async Task<bool> Exists(int id) {
            return await context.Movies.AnyAsync(m => m.id == id);
        }

        public async Task<int> Create(Movie movie) {
            context.Movies.Add(movie);
            await context.SaveChangesAsync();
            return context.SaveChanges();
        }

        public async Task Update(Movie movie) {
            context.Update(movie);
            await context.SaveChangesAsync();
        }

        public async Task Delete(int id) {
            await context.Movies.Where(m => m.id == id).ExecuteDeleteAsync();
        }
    }
}
