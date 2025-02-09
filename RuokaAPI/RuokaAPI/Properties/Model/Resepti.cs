using RuokaAPI.Dtos;
using RuokaAPI.Properties.Model;
using System.Collections.Generic;

public class ReseptiValidator
{
    private const int MaxValmistuskuvausLength = 1024;
    private const int MaxAinesosaLength = 30;

    public List<string> Validate(ReseptiDto reseptiDto)
    {
        var validationMessages = new List<string>();

        ValidateValmistuskuvaus(reseptiDto.Valmistuskuvaus, validationMessages);
        ValidateAinesosat(reseptiDto.Ainesosat, validationMessages);
        ValidateAvainsanat(reseptiDto.Avainsanat, validationMessages);

        return validationMessages;
    }

   
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

namespace RuokaAPI.Properties.Model
{
    public class Resepti
    {
        public int Id { get; set; }
        public int Tekijäid { get; set; }
        public string Nimi { get; set; }
        public List<Ainesosa> Ainesosat { get; set; } = new();
        public string? Valmistuskuvaus { get; set; }
        public string? Kuva1 { get; set; }
        public string? Kuva2 { get; set; }
        public string? Kuva3 { get; set; }
        public string? Kuva4 { get; set; }
        public string? Kuva5 { get; set; }
        public string? Kuva6 { get; set; }
        public List<Avainsana> Avainsanat { get; set; } = new();
        public List<Arvostelu> Arvostelut { get; set; } = new();
        public string? Katseluoikeus { get; set; }
    }
}
