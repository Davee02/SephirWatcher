using System.Threading.Tasks;
using DaHo.SephirWatcher.Models;
using RestEase;

namespace DaHo.SephirWatcher.Interfaces
{
    [Header("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/74.0.3729.108 Safari/537.36")]
    public interface ISephirApi
    {
        [Get("login.cfm")]
        Task<string> Index();


        [Get("40_berufsfachschule/noten.cfm")]
        Task<string> MarksOverview([Query] string cfId, [Query] string cfToken);

        [Get("40_berufsfachschule/noten.cfm")]
        Task<string> Marks([Query] string cfId, [Query] string cfToken, [Query(Name = "fl_open")] string subjectNumber = "all");

        [Post("login_action.cfm")]
        Task Login([Body] SephirAccount account, [Query] string cfId, [Query] string cfToken);
    }
}
