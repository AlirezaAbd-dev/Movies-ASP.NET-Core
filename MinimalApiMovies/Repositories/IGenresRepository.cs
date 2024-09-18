using MinimalApiMovies.Entities;
using System.Linq.Expressions;

namespace MinimalApiMovies.Repositories {
    public interface IGenresRepository {
        Task<int> Create(Genre genre);
        Task<Genre?> GetById(int id);
        Task<List<Genre>> GetAll();
        Task<bool> Exists(int id);
        Task<List<int>> Exists(List<int> ids);
        Task Update(Genre genre);
        Task Delete(int id);
        Task<bool> Exists(int id, string name);
    }
}
