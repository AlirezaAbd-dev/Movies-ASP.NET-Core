namespace MinimalApiMovies.Entities {
    public class Movie {
        public int id {
            get; set;
        }
        public string Title { get; set; } = null!;
        public bool InTheaters {
            get; set;
        }
        public DateTime ReleaseDate {
            get; set;
        }
        public string? Poster {
            get; set;
        }
    }
}
