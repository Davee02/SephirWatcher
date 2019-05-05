using System.Collections.Generic;
using DaHo.SephirWatcher.Web.Models;

namespace DaHo.SephirWatcher.Web.Helper
{
    public class SephirTestWithoutIdsComparer : IEqualityComparer<SephirTest>
    {
        public bool Equals(SephirTest x, SephirTest y)
        {
            if (ReferenceEquals(null, x) || ReferenceEquals(null, y))
                return false;
            if (ReferenceEquals(x, y))
                return true;

            return x.ExamDate.Equals(y.ExamDate) && 
                   string.Equals(x.SchoolSubject, y.SchoolSubject) &&
                   string.Equals(x.ExamTitle, y.ExamTitle) && 
                   string.Equals(x.ExamState, y.ExamState) &&
                   string.Equals(x.MarkType, y.MarkType) && 
                   x.MarkWeighting.Equals(y.MarkWeighting) &&
                   x.Mark.Equals(y.Mark);
        }

        public int GetHashCode(SephirTest obj)
        {
            unchecked
            {
                var hashCode = obj.ExamDate.GetHashCode();
                hashCode = (hashCode * 397) ^ (obj.SchoolSubject != null ? obj.SchoolSubject.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.ExamTitle != null ? obj.ExamTitle.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.ExamState != null ? obj.ExamState.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (obj.MarkType != null ? obj.MarkType.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ obj.MarkWeighting.GetHashCode();
                hashCode = (hashCode * 397) ^ obj.Mark.GetHashCode();
                return hashCode;

            }
        }
    }
}