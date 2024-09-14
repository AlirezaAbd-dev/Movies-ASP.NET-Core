using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Repositories {
    public class MoviesRepository(IHttpContextAccessor httpContextAccessor, ApplicationDbContext context, IMapper mapper) : IMoviesRepository {
        public async Task<List<Movie>> GetAllMovies(PaginationDTO pagination) {
            var queryable = context.Movies.AsQueryable();
            await httpContextAccessor.HttpContext!
                .InsertPaginationParameterInResponseHeader(queryable);
            return await queryable.Paginate(pagination).OrderBy(m => m.Title).ToListAsync();
        }

        public async Task<Movie?> GetById(int id) {
            return await context.Movies
                .Include(m => m.Comments)
                .Include(m => m.GenresMovies)
                    .ThenInclude(gm => gm.Genre)
                .Include(m => m.ActorsMovies.OrderBy(am => am.Order))
                    .ThenInclude(gm => gm.Actor)
                .AsNoTracking().FirstOrDefaultAsync(m => m.id == id);
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

        public async Task Assign(int id, List<int> genresId) {
            var movie = await context.Movies.Include(m => m.GenresMovies)
                .FirstOrDefaultAsync(m => m.id == id);

            if( movie is null ) {
                throw new ArgumentException($"There is no movie with id {id}");
            }

            var genresMovies = genresId.Select(genreId => new GenreMovie { GenreId = genreId });

            movie.GenresMovies = mapper.Map(genresMovies, movie.GenresMovies);

            await context.SaveChangesAsync();
        }

        public async Task Assign(int id, List<ActorMovie> actors) {
            for( int i = 0; i <= actors.Count; i++ ) {
                actors[i].Order = i;
            }

            var movie = await context.Movies.Include(m => m.ActorsMovies)
                .FirstOrDefaultAsync(m => m.id == id);

            if( movie is null ) {
                throw new ArgumentException($"There is no movie with id {id}");
            }

            movie.ActorsMovies = mapper.Map(actors, movie.ActorsMovies);
            await context.SaveChangesAsync();
        }
    }
}
