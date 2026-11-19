using System.ComponentModel.DataAnnotations;

namespace KickZone.Models.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Въведи потребителско име или имейл.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Въведи парола.")]
        public string Password { get; set; } = string.Empty;

        public string? ReturnUrl { get; set; }
        public string? Error { get; set; }
    }
}
