using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class UpdateUserRequest
    {
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Ime mora imati između 2 i 100 znakova.")]
        public string? FirstName { get; set; }

        [StringLength(100, MinimumLength = 2, ErrorMessage = "Prezime mora imati između 2 i 100 znakova.")]
        public string? LastName { get; set; }

        [StringLength(50, MinimumLength = 3, ErrorMessage = "Korisničko ime mora imati između 3 i 50 znakova.")]
        public string? Username { get; set; }

        [EmailAddress(ErrorMessage = "Email adresa nije ispravna.")]
        public string? Email { get; set; }

        [RegularExpression(@"^(\+387|0)\s?6[0-9]\s?[0-9]{3}\s?[0-9]{3,4}$",
            ErrorMessage = "Neispravan format broja telefona (npr. +387 61 234 567).")]
        public string? PhoneNumber { get; set; }

        public string? ProfileImageUrl { get; set; }
    }
}
