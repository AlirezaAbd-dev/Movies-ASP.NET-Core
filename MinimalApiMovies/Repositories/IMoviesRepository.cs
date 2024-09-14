using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Repositories {
    public interface IMoviesRepository {
        Task Assign(int id, List<int> genresId);
        Task Assign(int id, List<ActorMovie> actors);
        Task<int> Create(Movie movie);
        Task Delete(int id);
        Task<bool> Exists(int id);
        Task<List<Movie>> GetAllMovies(PaginationDTO pagination);
        Task<Movie?> GetById(int id);
        Task Update(Movie movie);
    }
}