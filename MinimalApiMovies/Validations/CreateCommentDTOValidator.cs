using FluentValidation;
using MinimalApiMovies.DTOs;

namespace MinimalApiMovies.Validations {
    public class CreateCommentDTOValidator:AbstractValidator<CreateCommentDTO> {
        public CreateCommentDTOValidator()
        {
            RuleFor(p=> p.Body).NotEmpty().WithMessage(ValidationUtilities.NonEmptyMessage);
        }
    }
}
