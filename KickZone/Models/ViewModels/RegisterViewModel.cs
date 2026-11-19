using System.ComponentModel.DataAnnotations;

namespace KickZone.Models.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Потребителското име е задължително.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Имейлът е задължителен.")]
        [EmailAddress(ErrorMessage = "Невалиден имейл.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Паролата е задължителна.")]
        [MinLength(6, ErrorMessage = "Паролата трябва да е поне 6 символа.")]
        public string Password { get; set; } = string.Empty;

        [Compare(nameof(Password), ErrorMessage = "Паролите не съвпадат.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? Error { get; set; }
    }
}
