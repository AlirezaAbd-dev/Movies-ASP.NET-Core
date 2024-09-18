using AutoMapper;
using FluentValidation;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;
using MinimalApiMovies.Filters;
using MinimalApiMovies.Repositories;
using MinimalApiMovies.Services;

namespace MinimalApiMovies.Endpoints {
    public static class ActorsEndpoints {
        private static readonly string container = "actors";

        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group) {
            group.MapGet("/", GetAll).CacheOutput(c => c.Expire(TimeSpan.FromMinutes(1)).Tag("actors-get"));
            group.MapPost("/", Create).AddEndpointFilter<ValidationFilter<CreateActorDTO>>().DisableAntiforgery();
            group.MapGet("/getByName/{name}", GetByName);
            group.MapGet("/{id:int}", GetById);
            group.MapPut("/{id:int}", Update).AddEndpointFilter<ValidationFilter<CreateActorDTO>>().DisableAntiforgery();
            group.MapDelete("/{id:int}", Delete);
            return group;
        }

        static async Task<Ok<List<ActorDTO>>> GetAll(IActorsRepository repository, IMapper mapper, int page = 1, int recordsPerPage = 10) {
            var pagination = new PaginationDTO() { Page = page, RecorsPerPage = recordsPerPage };
            var actors = await repository.GetAll(pagination);
            var actorsDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDTO);
        }

        static async Task<Ok<List<ActorDTO>>> GetByName(string name, IActorsRepository repository, IMapper mapper) {
            var actors = await repository.GetByName(name);
            var actorsDTO = mapper.Map<List<ActorDTO>>(actors);
            return TypedResults.Ok(actorsDTO);
        }

        static async Task<Results<Ok<ActorDTO>, NotFound>> GetById(int id, IActorsRepository repository, IMapper mapper) {
            var actor = await repository.GetById(id);

            if( actor is null ) {
                return TypedResults.NotFound();
            }

            var actorDTO = mapper.Map<ActorDTO>(actor);
            return TypedResults.Ok(actorDTO);
        }

        static async Task<Created<ActorDTO>> Create(
            [FromForm] CreateActorDTO createActorDTO, 
            IFormFile file,
            IActorsRepository repository, IOutputCacheStore outputCacheStore,
            IMapper mapper, IFileStorage fileStorage,
            IValidator<CreateActorDTO> validator
            ) {
            var actor = mapper.Map<Actor>(createActorDTO);

            if( file is not null ) {
                var url = await fileStorage.Store(container, file);
                actor.Picture = url;
            }

            var id = await repository.Create(actor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            var actorDto = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actors/{id}", actorDto);
        }

        static async Task<Results<NoContent, NotFound>> Update(int id, [FromForm] CreateActorDTO createActorDTO, IFormFile? picture, IActorsRepository repository, IFileStorage localFileStorage, IOutputCacheStore outputCacheStore, IMapper mapper) {
            var actorDB = await repository.GetById(id);

            if( actorDB is null ) {
                return TypedResults.NotFound();
            }

            var actorForUpdate = mapper.Map<Actor>(createActorDTO);
            actorForUpdate.Id = id;
            actorForUpdate.Picture = actorDB.Picture;

            if( picture is not null ) {
                var url = await localFileStorage.Edit(actorForUpdate.Picture, container, picture);
                actorForUpdate.Picture = url;
            }

            await repository.Update(actorForUpdate);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }

        static async Task<Results<NoContent, NotFound>> Delete(int id, IActorsRepository repository, IOutputCacheStore outputCacheStore,IFileStorage fileStorage) {
            var actorDB = await repository.GetById(id);

            if( actorDB is null ) {
                return TypedResults.NotFound();
            }

            await repository.Delete(id);
            await fileStorage.Delete(actorDB.Picture,container);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            return TypedResults.NoContent();
        }
    }
}
