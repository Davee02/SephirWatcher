using System;
using System.Threading.Tasks;
using DaHo.SephirWatcher.Interfaces;
using RestEase;

namespace DaHo.SephirWatcher
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var api = RestClient.For<ISephirApi>("https://sephir.ch/ICT/user/lernendenportal");
            var indexPage = await api.Index();
            var marks = await api.MarksOverview("15008178", "532ba0cd52c2d687-12C87041-5056-B23F-B591B8FA02F08908");
            var physicMarks = await api.Marks("15008178", "532ba0cd52c2d687-12C87041-5056-B23F-B591B8FA02F08908");
        }
    }
}
