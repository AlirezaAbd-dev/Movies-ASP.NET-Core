using System.ComponentModel.DataAnnotations;
using System.Diagnostics.CodeAnalysis;

namespace MinimalApiMovies.DTOs {
    public class CreateGenreDTO {
        [MaxLength(150)]
        public string Name { get; set; } = null!;
    }
}
