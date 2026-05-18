using System.ComponentModel.DataAnnotations;

namespace WorldApp.Models;

public class City
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "El nombre de la ciudad es obligatorio")]
    [StringLength(35, ErrorMessage = "Máximo 35 caracteres")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Debe seleccionar un país")]
    [StringLength(3)]
    public string CountryCode { get; set; } = string.Empty;

    [Required]
    [StringLength(20)]
    public string District { get; set; } = string.Empty;

    [Required]
    [Range(0, int.MaxValue, ErrorMessage = "La población no puede ser negativa")]
    public int Population { get; set; }

    // Relación
    public virtual Country? Country { get; set; }

    // Soft delete
    public bool IsDeleted { get; set; } = false;
}