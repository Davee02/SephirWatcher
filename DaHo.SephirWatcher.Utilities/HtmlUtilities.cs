using System.Collections.Generic;
using System.Text;

namespace DaHo.SephirWatcher.Utilities
{
    public static class HtmlUtilities
    {
        public static string CreateUnorderedList(IEnumerable<object> items)
        {
            var sb = new StringBuilder();
            sb.Append("<ul>");

            foreach (var item in items)
            {
                sb.Append($"<li>{item}</li>");
            }

            sb.Append("</ul>");

            return sb.ToString();
        }
    }
}
