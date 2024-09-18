namespace MinimalApiMovies.Validations {
    public static class ValidationUtilities {
        public static string NonEmptyMessage = "The field {PropertyName} is required";
        public static string MaximumLengthMessage = "The field {PropertyName} should bew less than {MaxLength} characters";
        public static string FirstLetterIsUpperCaseMessage = "the field {PropertyName} should start with uppercase";


        public static bool FirstLetterIsUpperCase(string value) {
            if( string.IsNullOrWhiteSpace(value) ) {
                return true;
            }

            var firstLetter = value[0].ToString();
            return firstLetter == firstLetter.ToUpper();
        }

        public static string GeaterThanDate(DateTime value) => "The Field {PropertyName} should be greater than " + value.ToString("yyyy-MM-dd");
    }
}
