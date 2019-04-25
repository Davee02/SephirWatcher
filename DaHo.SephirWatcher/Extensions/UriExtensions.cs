using System;

namespace DaHo.SephirWatcher.Extensions
{
    public static class UriExtensions
    {
        /// <summary>
        /// Returns an Uri-object with a dummy hostname and the <see cref="queryString" />
        /// </summary>
        /// <param name="queryString">A valid url action with optional url-encoded parameter(s). Example: "login_action.cfm?cfid=123&amp;cftoken=abcd"</param>
        /// <returns>An Uri-object with the absolute uri-path "http://www.dummy.net/"</returns>
        public static Uri CreateDummyUriFromActionAndQuery(this string queryString)
        {
            return new Uri($"http://www.dummy.net/{queryString}");
        }
    }
}
