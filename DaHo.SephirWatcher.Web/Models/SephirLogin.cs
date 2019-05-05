using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Identity;

namespace DaHo.SephirWatcher.Web.Models
{
    public class SephirLogin
    {
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Key]
        public long Id { get; set; }
        
        [Required]
        public string EmailAdress { get; set; }


        [Required]
        public string EncryptedPassword { get; set; }

        public IdentityUser IdentityUser { get; set; }

        [Required]
        public string IdentityUserId { get; set; }
    }
}
