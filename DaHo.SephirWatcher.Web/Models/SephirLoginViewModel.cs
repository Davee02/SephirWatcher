using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DaHo.SephirWatcher.Web.Models
{
    public class SephirLoginViewModel
    {
        [DisplayName("Email address")]
        public string EmailAdress { get; set; }

        [Required(ErrorMessage = "Bitte gib das Passwort an")]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }
    }
}
