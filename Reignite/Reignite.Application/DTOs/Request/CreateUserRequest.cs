using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CreateUserRequest
    {
        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ime mora imati između 2 i 100 znakova.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prezime mora imati između 2 i 100 znakova.")]
        public string LastName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 50 znakova.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Email adresa nije ispravna.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati najmanje 6 znakova.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Broj telefona je obavezan.")]
        [RegularExpression(@"^(\+387|0)\s?6[0-9]\s?[0-9]{3}\s?[0-9]{3,4}$",
            ErrorMessage = "Neispravan format broja telefona (npr. +387 61 234 567).")]
        public string PhoneNumber { get; set; } = string.Empty;
    }
}
