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
        /// <returns>The html code of the page</returns>
        [Get("login.cfm")]
        Task<string> Index();


        /// <summary>
        /// Makes a POST request to the login page of sephir
        /// </summary>
        /// <param name="accountInfo">
        /// A dictionary with 2 key-value-pairs.
        /// The 2 key must be 'email' and 'passwort' and the values the corresponding string-values
        /// </param>
        /// <param name="cfId">The session-id</param>
        /// <param name="cfToken">The session-token</param>
        /// <returns></returns>
        [Post("login_action.cfm")]
        Task<string> Login([Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> accountInfo, [Query] string cfId, [Query] string cfToken);
    }
}
