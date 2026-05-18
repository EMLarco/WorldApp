using System.ComponentModel.DataAnnotations;

namespace WorldApp.Models;

public class RegisterViewModel
{
    [Required(ErrorMessage = "El email es obligatorio")]
    [EmailAddress(ErrorMessage = "Email inválido")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "La contraseña es obligatoria")]
    [StringLength(100, MinimumLength = 6, ErrorMessage = "Mínimo 6 caracteres")]
    [DataType(DataType.Password)]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Confirme la contraseña")]
    [Compare("Password", ErrorMessage = "Las contraseñas no coinciden")]
    [DataType(DataType.Password)]
    public string ConfirmPassword { get; set; } = string.Empty;
}