using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Repositories;

public interface IActorsRepository
{
    Task<List<Actor>> GetAll(PaginationDTO pagination);
    Task<Actor?> GetById(int id);
    Task<List<Actor>> GetByName(string name);
    Task<int> Create(Actor actor);
    Task<bool> Exists(int id);
    Task Update(Actor actor);
    Task Delete(int id);
}