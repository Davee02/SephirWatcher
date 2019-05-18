using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;

namespace DaHo.SephirWatcher.Web.Pinger
{
    public static class SephirWatcherPinger
    {
        [FunctionName("SephirWatcherPinger")]
        public static async Task Run([TimerTrigger("0 */5 * * * *")]TimerInfo myTimer, ILogger log)
        {
            log.LogInformation($"C# Timer trigger function started at: {DateTime.Now}");

            using (var client = new HttpClient())
            {
                var response = await client.GetAsync("https://sephirwatcher.azurewebsites.net/");

                if (response.IsSuccessStatusCode)
                    log.LogInformation($"Ping ended successfully with the message: {response.ReasonPhrase}");
                else
                    log.LogError($"Ping ended not successfully with the http-code: { response.StatusCode}");
            }
        }
    }
}
