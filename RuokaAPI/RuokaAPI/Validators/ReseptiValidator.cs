using RuokaAPI.Dtos;

// This class is responsible for validating the properties of a ReseptiRequest DTO.
// It ensures that the description, ingredients, and keywords meet specific criteria such as length and non-null values.
public class ReseptiValidator
{
    private const int MaxValmistuskuvausLength = 1024;
    private const int MaxAinesosaLength = 30;

    // Validates the given ReseptiRequest DTO and returns a list of validation messages.
    public List<string> Validate(ReseptiRequest reseptiDto)
    {
        var validationMessages = new List<string>();

        ValidateValmistuskuvaus(reseptiDto.Valmistuskuvaus, validationMessages);
        ValidateAinesosat(reseptiDto.Ainesosat.Select(a => a.Ainesosa).ToArray(), validationMessages);
        ValidateAvainsanat(reseptiDto.Avainsanat, validationMessages);

        return validationMessages;
    }

    // Validates the description of the recipe and adds validation messages if any issues are found.
    private void ValidateValmistuskuvaus(string valmistuskuvaus, List<string> validationMessages)
    {
        if (string.IsNullOrWhiteSpace(valmistuskuvaus))
        {
            validationMessages.Add("Valmistuskuvaus cannot be null or empty.");
        }
        if (valmistuskuvaus.Length > MaxValmistuskuvausLength)
        {
            validationMessages.Add($"Valmistuskuvaus cannot exceed {MaxValmistuskuvausLength} characters.");
        }
    }

    // Validates the ingredients of the recipe and adds validation messages if any issues are found.
    private void ValidateAinesosat(string[] ainesosat, List<string> validationMessages)
    {
        if (ainesosat != null)
        {
            foreach (var ainesosa in ainesosat)
            {
                if (string.IsNullOrWhiteSpace(ainesosa))
                {
                    validationMessages.Add("Ainesosa cannot be null or empty.");
                }
                if (ainesosa.Length > MaxAinesosaLength)
                {
                    validationMessages.Add($"Ainesosa cannot exceed {MaxAinesosaLength} characters.");
                }
            }
        }
    }

    // Validates the keywords of the recipe and adds validation messages if any issues are found.
    private void ValidateAvainsanat(string[] avainsanat, List<string> validationMessages)
    {
        if (avainsanat != null)
        {
            foreach (var avainsana in avainsanat)
            {
                if (string.IsNullOrWhiteSpace(avainsana))
                {
                    validationMessages.Add("Avainsana cannot be null or empty.");
                }
                if (avainsana.Length > MaxAinesosaLength)
                {
                    validationMessages.Add($"Avainsana cannot exceed {MaxAinesosaLength} characters.");
                }
            }
        }
    }
}
