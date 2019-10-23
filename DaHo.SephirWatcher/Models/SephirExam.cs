using System;

namespace DaHo.SephirWatcher.Models
{
    public class SephirExam
    {
        public DateTime ExamDate { get; set; }

        public string SchoolSubject { get; set; }

        public string ExamTitle { get; set; }

        public string ExamState { get; set; }

        public string MarkType { get; set; }

        public double? MarkWeighting { get; set; }

        public double? Mark { get; set; }

        public string ExamId { get; set; }
    }
}
