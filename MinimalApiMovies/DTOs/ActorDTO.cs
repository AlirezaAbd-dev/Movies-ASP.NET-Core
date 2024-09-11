namespace MinimalApiMovies.DTOs {
    public class ActorDTO {
        public int Id { get; set; }
        public string Name {
            get;
            set;
        } = null!;

        public DateTime DateOfBirth {
            get; set;
        }

        public string? Picture {
            get; set;
        }
    }
}
