namespace WorldApp.Models;

public class CountryLanguage
{
    public string CountryCode { get; set; } = string.Empty;
    public string Language { get; set; } = string.Empty;
    public string IsOfficial { get; set; } = "F";   // 'T' o 'F'
    public decimal Percentage { get; set; }

    // Relación
    public virtual Country Country { get; set; } = null!;

    // Soft delete
    public bool IsDeleted { get; set; } = false;
}