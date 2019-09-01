using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DaHo.SephirWatcher.Web.Models
{
    public class SephirTest
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public long Id { get; set; }

        [DataType(DataType.Date)]
        [Required]
        public DateTime ExamDate { get; set; }

        [Required]
        public string SchoolSubject { get; set; }

        [Required]
        public string ExamTitle { get; set; }

        [Required]
        public string MarkType { get; set; }

        [Range(0, 100)]
        public double? MarkWeighting { get; set; }

        public string EncryptedMark { get; set; }

        [NotMapped]
        public string Mark { get; set; }

        public SephirLogin SephirLogin { get; set; }

        [Required]
        public long SephirLoginId { get; set; }

        public override string ToString() => $"{SchoolSubject}: {ExamTitle} ({Mark})";
    }
}