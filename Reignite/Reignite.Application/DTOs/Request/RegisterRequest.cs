using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email je obavezan.")]
        [EmailAddress(ErrorMessage = "Unesite ispravnu email adresu.")]
        [RegularExpression(@"^[^\s@]+@[^\s@]+\.[^\s@]+$", ErrorMessage = "Unesite ispravan format email adrese.")]
        [StringLength(50, ErrorMessage = "Email ne smije biti duži od 50 znakova.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Lozinka je obavezna.")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Lozinka mora imati minimalno 6 karaktera.")]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Korisničko ime je obavezno.")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 50 znakova.")]
        public string Username { get; set; } = string.Empty;

        [Required(ErrorMessage = "Broj telefona je obavezan.")]
        [RegularExpression(@"^(\+387|0)\s?6[0-9]\s?[0-9]{3}\s?[0-9]{3,4}$",
            ErrorMessage = "Neispravan format broja telefona (npr. +387 61 234 567).")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Ime je obavezno.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ime ne smije biti prazno.")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Prezime je obavezno.")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prezime ne smije biti prazno.")]
        public string LastName { get; set; } = string.Empty;
    }
}

