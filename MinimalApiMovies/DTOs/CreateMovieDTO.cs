namespace MinimalApiMovies.DTOs {
    public class CreateMovieDTO {
        public string Title {
            get; set;
        } = null!;
        public bool InTeaters {
            get; set;
        }
        public DateTime ReleaseDate {
            get; set;
        }
    }
}
