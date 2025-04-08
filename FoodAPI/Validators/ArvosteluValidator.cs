using FoodAPI.Properties.Model;

public class ArvosteluValidator
    {
        private const int MaxKommenttiLength = 500;
        private const int MinRating = 1;
        private const int MaxRating = 5;

        public List<string> Validate(ArvosteluRequest request)
        {
            var validationMessages = new List<string>();

            if (request == null)
            {
                validationMessages.Add("Arvostelu cannot be null.");
                return validationMessages;
            }

            if (request.Numeroarvostelu < MinRating || request.Numeroarvostelu > MaxRating)
            {
                validationMessages.Add($"Rating must be between {MinRating} and {MaxRating}.");
            }

            if (string.IsNullOrWhiteSpace(request.Vapaateksti))
            {
                validationMessages.Add("Kommentti cannot be null or empty.");
            }
            else if (request.Vapaateksti.Length > MaxKommenttiLength)
            {
                validationMessages.Add($"Kommentti cannot exceed {MaxKommenttiLength} characters.");
            }

            return validationMessages;
        }
    }
