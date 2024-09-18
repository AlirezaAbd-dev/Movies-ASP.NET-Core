using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Repositories {
    public interface IErrorsRepository {
        Task Create(Error error);
    }
}