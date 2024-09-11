using AutoMapper;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;
using MinimalApiMovies.Repositories;
using MinimalApiMovies.Services;

namespace MinimalApiMovies.Endpoints {
    public static class ActorsEndpoints {
        private static readonly string container = "actors";

        public static RouteGroupBuilder MapActors(this RouteGroupBuilder group) {
            group.MapPost("/", Create).DisableAntiforgery();
            return group;
        }

        static async Task<Created<ActorDTO>> Create([FromForm] CreateActorDTO createActorDTO,
            IActorsRepository repository, IOutputCacheStore outputCacheStore,
            IMapper mapper, IFileStorage fileStorage
            ) {
            var actor = mapper.Map<Actor>(createActorDTO);

            //if( createActorDTO.Picture is not null ) {
            //    var url = await fileStorage.Store(container, createActorDTO.Picture);
            //    actor.Picture = url;
            //}

            var id = await repository.Create(actor);
            await outputCacheStore.EvictByTagAsync("actors-get", default);
            var actorDto = mapper.Map<ActorDTO>(actor);
            return TypedResults.Created($"/actors/{id}", actorDto);
        }
    }
}
