﻿using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;
using MinimalApiMovies.Filters;
using MinimalApiMovies.Migrations;
using MinimalApiMovies.Repositories;
using MinimalApiMovies.Services;

namespace MinimalApiMovies.Endpoints {
    public static class MoviesEndpoints {
        private readonly static string container = "movies";
        public static RouteGroupBuilder MapMovies(this RouteGroupBuilder group) {
            group.MapGet("/", GetAll)
                .CacheOutput(c => c.Expire(TimeSpan.FromSeconds(60)).Tag("movies-get"));
            group.MapGet("/{id:int}", GetById);
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateMovieDTO>>().DisableAntiforgery();
            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateMovieDTO>>().DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);
            group.MapPost("/{id:int}/assignGenres", AssignGenres);
            group.MapPost("/{id:int}/assignActors", AssignActors);
            return group;
        }

        static async Task<Ok<List<MovieDTO>>> GetAll(IMoviesRepository repository,
            IMapper mapper, int page = 1, int recordsPerPage = 10) {
            var pagination = new PaginationDTO { Page = page, RecorsPerPage = recordsPerPage };
            var movies = await repository.GetAllMovies(pagination);
            var movieDTO = mapper.Map<List<MovieDTO>>(movies);

            return TypedResults.Ok(movieDTO);
        }

        static async Task<Results<Ok<MovieDTO>, NotFound>> GetById(int id, IMoviesRepository repository, IMapper mapper) {
            var movie = await repository.GetById(id);

            if( movie == null ) {
                return TypedResults.NotFound();
            }

            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Ok(movieDTO);
        }

        static async Task<Created<MovieDTO>> Create([FromForm] CreateMovieDTO createMovieDTO,
            IFormFile? Poster,
            IMoviesRepository repository,
            IFileStorage fileStorage,
            IOutputCacheStore outputCacheStore,
            IMapper mapper
            ) {
            var movie = mapper.Map<Movie>(createMovieDTO);

            if( Poster is not null ) {
                var url = await fileStorage.Store(container, Poster);
                movie.Poster = url;
            }

            var id = await repository.Create(movie);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            var movieDTO = mapper.Map<MovieDTO>(movie);
            return TypedResults.Created($"movies/{id}", movieDTO);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id,
             [FromForm] CreateMovieDTO createMovieDTO,
            IFormFile? Poster,
             IMoviesRepository repository,
             IFileStorage fileStorage,
             IMapper mapper,
             IOutputCacheStore outputCacheStore
            ) {
            var movieDB = await repository.GetById(id);

            if( movieDB == null ) {
                return TypedResults.NotFound();
            }

            var movieForUpdate = mapper.Map<Movie>(createMovieDTO);
            movieForUpdate.id = id;
            movieForUpdate.Poster = movieDB.Poster;

            if( Poster is not null ) {
                var url = await fileStorage.Edit(movieForUpdate.Poster, container, Poster);
                movieForUpdate.Poster = url;
            }

            await repository.Update(movieForUpdate);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent>> Delete(int id, IMoviesRepository repository, IFileStorage fileStorage, IOutputCacheStore outputCacheStore) {
            var MovieDB = await repository.GetById(id);

            if( MovieDB is null ) {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await fileStorage.Delete(MovieDB.Poster, container);
            await outputCacheStore.EvictByTagAsync("movies-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound, BadRequest<string>>> AssignGenres(int id,
            List<int> genresIds,
            IMoviesRepository moviesRepository,
            IGenresRepository genresRepository
            ) {
            if( !await moviesRepository.Exists(id) ) {
                return TypedResults.NotFound();
            }

            var existingGenres = new List<int>();

            if( genresIds.Count != 0 ) {
                existingGenres = await genresRepository.Exists(genresIds);
            }

            if( genresIds.Count != existingGenres.Count ) {
                var nonExistingGenres = genresIds.Except(existingGenres);

                var nonExistingGenresCSV = string.Join(",", nonExistingGenres);

                return TypedResults.BadRequest($"The genres of id {nonExistingGenresCSV} does not exist.");
            }

            await moviesRepository.Assign(id, genresIds);
            return TypedResults.NoContent();
        }

        static async Task<Results<NotFound, NoContent, BadRequest<string>>> AssignActors
            (int id, List<AssignActorMovieDTO> actorsDTO, IMoviesRepository moviesRepository,
            IActorsRepository actorRepository, IMapper mapper) {
            if( !await moviesRepository.Exists(id) ) {
                return TypedResults.NotFound();
            }

            var existingActors = new List<int>();
            var actorsIds = actorsDTO.Select(a => a.ActorId).ToList();

            if( actorsDTO.Count != 0 ) {
                existingActors = await actorRepository.Exists(actorsIds);
            }

            if( existingActors.Count != actorsDTO.Count ) {
                var nonExistingActors = actorsIds.Except(existingActors);
                var nonExistingActorsCSV = string.Join(",", nonExistingActors);
                return TypedResults.BadRequest($"The actors of id {nonExistingActorsCSV} do not exist.");
            }

            var actors = mapper.Map<List<ActorMovie>>(actorsDTO);
            await moviesRepository.Assign(id, actors);
            return TypedResults.NoContent();
        }
    }
}
