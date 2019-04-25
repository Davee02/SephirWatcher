using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Web;
using DaHo.SephirWatcher.Extensions;
using DaHo.SephirWatcher.Interfaces;
using DaHo.SephirWatcher.Models;
using HtmlAgilityPack;
using RestEase;

namespace DaHo.SephirWatcher
{
    internal class Program
    {
        private static async Task Main(string[] args)
        {
            var api = RestClient.For<ISephirApi>("https://sephir.ch/ICT/user/lernendenportal");

            var indexPage = await api.Index();
            var tokens = GetCfAuthentificationFromIndexPage(indexPage);

            var loggedIn = await api.Login(SephirAccountToDictionary(new SephirAccount
            {
                AccountEmail = "david_hodel@sluz.ch",
                AccountPassword = "Error404!" 

            }), tokens.CfId, tokens.CfToken);

            Console.WriteLine(loggedIn);
        }

        private static CfAuthentification GetCfAuthentificationFromIndexPage(string indexPage)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(indexPage);

            var formAction = doc.QuerySelector("form[name=login]").GetAttributeValue("action", null);
            var actionParameters = HttpUtility.ParseQueryString(formAction.CreateDummyUriFromActionAndQuery().Query);

            return new CfAuthentification
            {
                CfId = actionParameters.Get("cfid"),
                CfToken = actionParameters.Get("cftoken")
            };

        }

        private static Dictionary<string, string> SephirAccountToDictionary(SephirAccount account)
        {
            return new Dictionary<string, string>(new []
            {
                new KeyValuePair<string, string>("email", account.AccountEmail),
                new KeyValuePair<string, string>("passwort", account.AccountPassword)
            });
        }
    }
}
