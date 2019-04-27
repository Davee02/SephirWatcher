using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DaHo.SephirWatcher.Web.Models
{
    public class SephirLoginViewModel
    {
        [Required(ErrorMessage = "Bitte gib die E-Mail Adresse an")]
        [EmailAddress(ErrorMessage = "Die E-Mail Adresse ist ungültig")]
        [DisplayName("E-Mail Adresse")]
        public string EmailAdress { get; set; }

        [Required(ErrorMessage = "Bitte gib das Passwort an")]
        [DataType(DataType.Password)]
        [DisplayName("Passwort")]
        public string Password { get; set; }
    }
}
