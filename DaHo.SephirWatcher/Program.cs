using System;
using System.Collections.Generic;
using System.Linq;
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

            var loggedIn = await api.Login(tokens.CfId, tokens.CfToken, SephirAccountToDictionary(new SephirAccount
            {
                AccountEmail = "david_hodel@sluz.ch",
                AccountPassword = "Error404!"

            }));

            var marks = await api.Marks(tokens.CfId, tokens.CfToken, GetDictionaryForMarks("12929"));
            var allExams = GetSephirExamsFromExamPage(marks).ToList();

            Console.WriteLine(marks);
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
            return new Dictionary<string, string>(new[]
            {
                new KeyValuePair<string, string>("email", account.AccountEmail),
                new KeyValuePair<string, string>("passwort", account.AccountPassword)
            });
        }

        private static Dictionary<string, string> GetDictionaryForMarks(string klasseId)
        {
            return new Dictionary<string, string>(new[]
            {
                new KeyValuePair<string, string>("klasseId", klasseId),
                new KeyValuePair<string, string>("klassefachId", "all"),
                new KeyValuePair<string, string>("seltyp", "klasse")
            });
        }

        private static IEnumerable<SephirExam> GetSephirExamsFromExamPage(string examPage)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(examPage);

            var tableRows = doc
                .QuerySelector("table.listtab_rot")
                .ChildNodes
                .Where(x => x.Name.Equals("tr"))
                .Skip(1)
                .Select(x => new
                {
                    Columns = x
                        .ChildNodes
                        .Where(y => y.Name.Equals("td"))
                        .Select(y => HttpUtility.HtmlDecode(y.InnerText)?.Trim())
                        .ToList() 

                })
                .ToList();


            foreach (var row in tableRows)
            {
                yield return new SephirExam
                {
                    ExamDate = DateTime.Parse(row.Columns[0]),
                    ExamState = row.Columns[3],
                    ExamTitle = row.Columns[2],
                    MarkType = row.Columns[4],
                    MarkWeighting = row.Columns[5].ParseOrNaN(),
                    Mark = row.Columns[6].ParseOrNaN(),
                    SchoolSubject = row.Columns[1]
                };
            }
        }
    }
}
