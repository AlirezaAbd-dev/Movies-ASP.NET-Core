namespace MinimalApiMovies.DTOs {
    public class PaginationDTO {
        public int Page {
            get; set;
        } = 1;
        private int recordsPerPage = 10;
        private readonly int recordsPerPageMax = 50;

        public int RecorsPerPage {
            get {
                return recordsPerPage;
            } set{
                if( value > recordsPerPageMax ) {
                    recordsPerPage = recordsPerPageMax;
                }
                else {
                    recordsPerPage = value;
                }
            } }
    }
}
