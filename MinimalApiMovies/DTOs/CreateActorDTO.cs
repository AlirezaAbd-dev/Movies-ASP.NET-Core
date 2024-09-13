namespace MinimalApiMovies.DTOs {
    public class CreateActorDTO
    {
        public string Name
        {
            get;
            set;
        } = null!;

        public DateTime DateOfBirth {
            get; set;
        }
    }
}
