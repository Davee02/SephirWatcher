using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace DaHo.SephirWatcher.Web.Models
{
    public class SephirLoginViewModel
    {
        [Required]
        [EmailAddress]
        [DisplayName("Email address")]
        public string EmailAdress { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [DisplayName("Password")]
        public string Password { get; set; }

        public long Id { get; set; }
    }
}
