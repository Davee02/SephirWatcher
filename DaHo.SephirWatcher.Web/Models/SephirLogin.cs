using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Newtonsoft.Json;

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
        [NotMapped]
        public string PlainPassword { get; set; }

        [JsonIgnore]
        [Required]
        public string EncryptedPassword { get; set; }
    }
}
