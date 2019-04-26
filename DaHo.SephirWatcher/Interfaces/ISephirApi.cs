using System.Collections.Generic;
using System.Threading.Tasks;
using RestEase;

namespace DaHo.SephirWatcher.Interfaces
{
    /// <summary>
    /// Provides endpoints for the sephir-webpage
    /// </summary>
    [Header("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.108 Safari/537.36")]
    public interface ISephirApi
    {
        /// <summary>
        /// Makes a GET request to the login page of sephir
        /// </summary>
        /// <returns>The html code of the login-page</returns>
        [Get("login.cfm")]
        Task<string> Index();


        /// <summary>
        /// Makes a POST request to the login page of sephir
        /// </summary>
        /// <param name="cfId">The session-id</param>
        /// <param name="cfToken">The session-token</param>
        /// <param name="accountInfo">
        /// A dictionary with 2 key-value-pairs. <para/>
        /// The 2 keys must be 'email' and 'passwort' and the values the corresponding string-values
        /// </param>
        /// <returns>
        /// If the login was successful: one line of javascript <para/>
        /// If the login was not successful: a html page which contains the message "Anmeldung nicht erfolgreich"
        /// </returns>
        [Post("login_action.cfm")]
        Task<string> Login([Query] string cfId, [Query] string cfToken, [Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> accountInfo);


        /// <summary>
        /// Makes a POST request to the page with all marks from the current semester and the provided class-id in <see cref="requestInfo"/>
        /// </summary>
        /// <param name="cfId">The session-id</param>
        /// <param name="cfToken">The session-token</param>
        /// <param name="requestInfo">
        /// A dictionary with 3 key-value-pairs. <para/>
        /// The 3 keys must be 'seltyp' (default value "klasse"), 'klassefachId' (default value "all") and 'klasseId'.
        /// </param>
        /// <returns>The html code of the marks-page in sephir</returns>
        [Post("40_berufsfachschule/noten.cfm?nogroup=pruef")]
        Task<string> Marks([Query] string cfId, [Query] string cfToken, [Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> requestInfo);
    }
}
