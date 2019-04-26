using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using DaHo.SephirWatcher.Extensions;
using DaHo.SephirWatcher.Interfaces;
using DaHo.SephirWatcher.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
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

            if (!WasLoginSuccessful(loggedIn))
            {
                await Console.Error.WriteLineAsync("Anmeldung war nicht erfolgreich. Bitte prüfe deine Daten.");
                return;
            }

            var classes = GetSchoolClassesFromMarksOverviewPage(await api.MarksOverview(tokens.CfId, tokens.CfToken)).ToList();
            var allExams = new List<SephirExam>();
            foreach (var schoolClass in classes)
            {
                var marks = await api.Marks(tokens.CfId, tokens.CfToken, GetDictionaryForMarks(schoolClass.ClassId));
                allExams.AddRange(GetSephirExamsFromExamPage(marks));
            }

            Console.WriteLine(allExams.Count);
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
            return new Dictionary<string, string>
            {
                {"email", account.AccountEmail},
                { "passwort", account.AccountPassword}
            };
        }

        private static Dictionary<string, string> GetDictionaryForMarks(string klasseId)
        {
            return new Dictionary<string, string>
            {
                { "klasseId", klasseId},
                {"klassefachId", "all"},
                {"seltyp", "klasse"}
            };
        }

        private static IEnumerable<SephirExam> GetSephirExamsFromExamPage(string examPage)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(examPage);

            var tableRows = doc
                .QuerySelectorAll("table.listtab_rot > tr:not(:first-child)")
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

        private static bool WasLoginSuccessful(string loginPage)
        {
            return !loginPage.Contains("Anmeldung nicht erfolgreich");
        }

        private static IEnumerable<SchoolClass> GetSchoolClassesFromMarksOverviewPage(string page)
        {
            var doc = new HtmlDocument();
            doc.LoadHtml(page);

            var classOptions = doc.QuerySelectorAll("select[name=klasseid] > option");

            foreach (var classOption in classOptions)
            {
                yield return new SchoolClass
                {
                    ClassId = classOption.GetAttributeValue("value", string.Empty),
                    ClassName = classOption.InnerText
                };
            }
        }
    }
}
