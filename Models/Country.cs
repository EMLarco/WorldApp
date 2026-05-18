using System.ComponentModel.DataAnnotations;

namespace WorldApp.Models;

public class Country
{
    [Key]
    [StringLength(3, MinimumLength = 3, ErrorMessage = "El código debe tener 3 caracteres")]
    public string Code { get; set; } = string.Empty;

    [Required(ErrorMessage = "El nombre del país es obligatorio")]
    [StringLength(52, ErrorMessage = "Máximo 52 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string Continent { get; set; } = string.Empty;

    [Required]
    [StringLength(26)]
    public string Region { get; set; } = string.Empty;

    [Required]
    [Range(0, double.MaxValue)]
    public decimal SurfaceArea { get; set; }

    public short? IndepYear { get; set; }

    [Required]
    [Range(0, int.MaxValue)]
    public int Population { get; set; }

    [Range(0, 100)]
    public decimal? LifeExpectancy { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? GNP { get; set; }

    [Range(0, double.MaxValue)]
    public decimal? GNPOld { get; set; }

    [Required]
    [StringLength(45)]
    public string LocalName { get; set; } = string.Empty;

    [Required]
    [StringLength(45)]
    public string GovernmentForm { get; set; } = string.Empty;

    [StringLength(60)]
    public string? HeadOfState { get; set; }

    public int? Capital { get; set; }

    [Required]
    [StringLength(2, MinimumLength = 2, ErrorMessage = "Código de 2 letras")]
    public string Code2 { get; set; } = string.Empty;

    // Relaciones
    public virtual ICollection<City> Cities { get; set; } = new List<City>();
    public virtual ICollection<CountryLanguage> Languages { get; set; } = new List<CountryLanguage>();

    // Soft delete
    public bool IsDeleted { get; set; } = false;
}