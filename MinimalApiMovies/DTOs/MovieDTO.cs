namespace MinimalApiMovies.DTOs {
    public class MovieDTO {
        public int id { get; set; }
        public string Title {
            get; set;
        } = null!;
        public bool InTeaters {
            get; set;
        }
        public DateTime ReleaseDate {
            get; set;
        }
        public string? Poster { get; set; }
    }
}
