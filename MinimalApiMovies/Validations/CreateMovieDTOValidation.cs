using FluentValidation;
using MinimalApiMovies.DTOs;

namespace MinimalApiMovies.Validations {
    public class CreateMovieDTOValidation:AbstractValidator<CreateMovieDTO> {
        public CreateMovieDTOValidation()
        {
            RuleFor(p=> p.Title).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage)
                .MaximumLength(250).WithMessage(ValidationUtilities.MaximumLengthMessage);
        }
    }
}
