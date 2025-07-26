namespace CurrencyConversionPortal.Api.DTOs
{
    using System.ComponentModel.DataAnnotations;

    public class LoginUserDto
    {
        [Required(ErrorMessage = "Username is required")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;
    }
}