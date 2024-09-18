using FluentValidation;
using MinimalApiMovies.DTOs;
using MinimalApiMovies.Repositories;

namespace MinimalApiMovies.Validations {
    public class CreateGenreDTOValidation : AbstractValidator<CreateGenreDTO> {
        public CreateGenreDTOValidation(IGenresRepository genreRepository, IHttpContextAccessor httpContextAccessor) {

            var routeValueID = httpContextAccessor.HttpContext!.Request.RouteValues["id"];
            var id = 0;

            if( routeValueID is string routeValueIdString ) {
                int.TryParse( routeValueIdString, out id );
            }

            RuleFor(p => p.Name)
                .NotEmpty()
                    .WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(150)
                    .WithMessage(ValidationUtilities.MaximumLengthMessage)
                .Must(ValidationUtilities.FirstLetterIsUpperCase)
                    .WithMessage(ValidationUtilities.FirstLetterIsUpperCaseMessage)
                .MustAsync(async (name, _) => {
                    var exists = await genreRepository.Exists(id, name);
                    return !exists;
                })
                    .WithMessage(g=> $"A genre with the name {g.Name} already exists");
        }
    }
}
