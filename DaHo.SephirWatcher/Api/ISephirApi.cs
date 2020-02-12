using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using RestEase;

namespace DaHo.SephirWatcher.Api
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
        [Get("ICT/user/lernendenportal/login.cfm")]
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
        [Post("ICT/user/lernendenportal/login_action.cfm")]
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
        [Post("ICT/user/lernendenportal/40_berufsfachschule/noten.cfm?nogroup=pruef")]
        Task<string> Marks([Query] string cfId, [Query] string cfToken, [Body(BodySerializationMethod.UrlEncoded)] IDictionary<string, string> requestInfo);


        /// <summary>
        /// Makes a GET request to the page with all marks from the current semester
        /// </summary>
        /// <param name="cfId">The session-id</param>
        /// <param name="cfToken">The session-token</param>
        /// <returns>The html code of the marks-page in sephir</returns>
        [Get("ICT/user/lernendenportal/40_berufsfachschule/noten.cfm")]
        Task<string> MarksOverview([Query] string cfId, [Query] string cfToken);


        /// <summary>
        /// Makes a GET request to the exam evaluation page
        /// </summary>
        /// <param name="cfId">The session-id</param>
        /// <param name="cfToken">The session-token</param>
        /// <param name="pruefungID">The id of the exam</param>
        /// <returns>The html code of the exam evaluation page in sephir</returns>
        [Get("ICT/user/lernendenportal/40_berufsfachschule/noten.cfm?act=pdet")]
        Task<string> ExamEvaluation([Query] string cfId, [Query] string cfToken, [Query] string pruefungID);


        /// <summary>
        /// Downloads the exam marks image
        /// </summary>
        /// <param name="imageName">The name of the image</param>
        /// <returns>The stream containing the image</returns>
        [Get("CFFileServlet/_cf_chart/{imageName}")]
        Task<Stream> ExamMarksChart([Path] string imageName);
    }
}
