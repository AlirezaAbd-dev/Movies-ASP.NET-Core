using AutoMapper;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Entities;

namespace MinimalApiMovies.Utilities {
    public class AutoMapperProfiles : Profile {
        public AutoMapperProfiles() {
            CreateMap<Genre, GenreDTO>();
            CreateMap<CreateGenreDTO, Genre>();

            CreateMap<Actor, ActorDTO>();
            CreateMap<CreateActorDTO, Actor>()
                .ForMember(p => p.Picture, opt => opt.Ignore());

            CreateMap<Movie, MovieDTO>()
                .ForMember(x => x.Genres, entity =>
                    entity.MapFrom(p =>
                    p.GenresMovies.Select(gm =>
                    new GenreDTO() { Id = gm.GenreId, Name = gm.Genre.Name })))
                .ForMember(x => x.Actors, entity =>
                    entity.MapFrom(p => p.ActorsMovies.Select(am => new ActorMovieDTO {
                        Id = am.ActorId,
                        Name = am.Actor.Name,
                        Character = am.Character
                    })));

            CreateMap<CreateMovieDTO, Movie>()
                .ForMember(p => p.Poster, opt => opt.Ignore());

            CreateMap<Comment, CommentDTO>();
            CreateMap<CreateCommentDTO, Comment>();

            CreateMap<AssignActorMovieDTO, ActorMovie>();
        }
    }
}
