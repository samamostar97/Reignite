using System.ComponentModel.DataAnnotations;

namespace Reignite.Application.DTOs.Request
{
    public class CreateUserAddressRequest
    {
        [Required(ErrorMessage = "Ulica je obavezna.")]
        [StringLength(200, ErrorMessage = "Ulica ne smije biti duža od 200 karaktera.")]
        public string Street { get; set; } = string.Empty;

        [Required(ErrorMessage = "Grad je obavezan.")]
        [StringLength(100, ErrorMessage = "Grad ne smije biti duži od 100 karaktera.")]
        public string City { get; set; } = string.Empty;

        [Required(ErrorMessage = "Poštanski broj je obavezan.")]
        [StringLength(20, ErrorMessage = "Poštanski broj ne smije biti duži od 20 karaktera.")]
        public string PostalCode { get; set; } = string.Empty;

        [Required(ErrorMessage = "Država je obavezna.")]
        [StringLength(100, ErrorMessage = "Država ne smije biti duža od 100 karaktera.")]
        public string Country { get; set; } = string.Empty;
    }
}
