using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;
using MinimalApiMovies.Repositories;

namespace MinimalApiMovies.Endpoints {
    public static class Genres {
        public static RouteGroupBuilder MapGenres(this RouteGroupBuilder group) {
            group.MapGet("/", GetGenres)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("genres-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create);
            group.MapPut("/{id:int}", Update);
            group.MapDelete("/{id:int}", Delete);

            return group;
        }

        static async Task<Ok<List<GenreDTO>>> GetGenres(IGenresRepository repository,IMapper mapper) {
            var genres = await repository.GetAll();
            var genresDTO = mapper.Map<List<GenreDTO>>(genres);
            return TypedResults.Ok(genresDTO);
        }

        static async Task<Results<Ok<GenreDTO>, NotFound<string>>> GetById(int Id, IGenresRepository repository,IMapper mapper) {
            var genre = await repository.GetById(Id);

            if( genre == null ) {
                return TypedResults.NotFound("Not Found!");
            }

            var genreDTO = mapper.Map<GenreDTO>(genre);

            return TypedResults.Ok(genreDTO);
        }

        static async Task<Created<GenreDTO>> Create(CreateGenreDTO createGenreDTO, IOutputCacheStore outputCacheStore, IGenresRepository repository, IMapper mapper) {
            var genre = mapper.Map<Genre>(createGenreDTO);
            var id = await repository.Create(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);

            var genreDTO = mapper.Map<GenreDTO>(genre);

            return TypedResults.Created($"/genres/{id}", genreDTO);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, CreateGenreDTO createGenreDTO, IGenresRepository repository, IOutputCacheStore outputCacheStore, IMapper mapper) {
            var genre = mapper.Map<Genre>(createGenreDTO);
            var exists = await repository.Exists(id);
            if( !exists ) {
                return TypedResults.NotFound();
            }

            genre.Id = id;
            await repository.Update(genre);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }

        static async Task<NoContent> Delete(int id, IGenresRepository repository, IOutputCacheStore outputCacheStore) {
            await repository.Delete(id);
            await outputCacheStore.EvictByTagAsync("genres-get", default);
            return TypedResults.NoContent();
        }
    }
}
