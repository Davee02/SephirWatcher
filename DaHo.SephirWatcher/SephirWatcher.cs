using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using DaHo.SephirWatcher.Extensions;
using DaHo.SephirWatcher.Api;
using DaHo.SephirWatcher.Models;
using HtmlAgilityPack;
using HtmlAgilityPack.CssSelectors.NetCore;
using RestEase;

namespace DaHo.SephirWatcher
{
    public class SephirWatcher
    {
        private readonly SephirAccount _account;
        private readonly ISephirApi _api;
        private CfAuthentification _tokens;
        private bool _isLoggedIn;

        public SephirWatcher(SephirAccount account)
        {
            _account = account;
            _api = RestClient.For<ISephirApi>("https://sephir.ch/ICT/user/lernendenportal");
        }

        public async Task<bool> AreCredentialsValid()
        {
            if (_isLoggedIn)
                return true;

            var tokens = await GetTokensAsync();
            var loginResult = await _api.Login(tokens.CfId, tokens.CfToken, CreateSephirAccountDictionary(_account));

            _isLoggedIn = true;

            return WasLoginSuccessful(loginResult);
        }

        public async Task<IList<SephirExam>> GetSephirExamsForAllClasses()
        {
            if (!_isLoggedIn && !await AreCredentialsValid())
            {
                throw new Exception("Could not log in to Sephir");
            }

            _isLoggedIn = true;
            var tokens = await GetTokensAsync();

            var classes = GetSchoolClassesFromMarksOverviewPage(await _api.MarksOverview(tokens.CfId, tokens.CfToken)).ToList();
            var allExams = new List<SephirExam>();
            foreach (var schoolClass in classes)
            {
                var marks = await _api.Marks(tokens.CfId, tokens.CfToken, CreateRequestInfoDictionary(schoolClass.ClassId));
                allExams.AddRange(GetSephirExamsFromExamPage(marks));
            }

            return allExams;
        }

        private async Task<CfAuthentification> GetTokensAsync()
        {
            if (_tokens == null)
            {
                var indexPage = await _api.Index();
                _tokens = GetCfAuthentificationFromIndexPage(indexPage);
            }

            return _tokens;
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

        private static Dictionary<string, string> CreateSephirAccountDictionary(SephirAccount account)
        {
            return new Dictionary<string, string>
            {
                { "email", account.AccountEmail },
                { "passwort", account.AccountPassword }
            };
        }

        private static Dictionary<string, string> CreateRequestInfoDictionary(string klasseId)
        {
            return new Dictionary<string, string>
            {
                { "klasseId", klasseId },
                { "klassefachId", "all" },
                { "seltyp", "klasse" }
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
                    ColumnTexts = x
                        .ChildNodes
                        .Where(y => y.Name.Equals("td"))
                        .Select(y => HttpUtility.HtmlDecode(y.InnerText)?.Trim())
                        .ToList(),
                    ColumnChildNodes = x
                        .ChildNodes
                        .Where(y => y.Name.Equals("td"))
                        .ToList()
                });


            foreach (var row in tableRows)
            {
                yield return new SephirExam
                {
                    ExamDate = DateTime.Parse(row.ColumnTexts[0], new CultureInfo("de-ch")),
                    SchoolSubject = row.ColumnTexts[1],
                    ExamTitle = row.ColumnTexts[2],
                    ExamState = row.ColumnTexts[3],
                    MarkType = row.ColumnTexts[4],
                    MarkWeighting = row.ColumnTexts[5].ParseOrDefault(null),
                    Mark = row.ColumnTexts[6].ParseOrDefault(null),
                    ExamId = GetExamIdFromSephirColumn(row.ColumnChildNodes[7])
                };
            }
        }

        private static string GetExamIdFromSephirColumn(HtmlNode nodes)
        {
            var hrefValue = nodes
                .ChildNodes
                .FirstOrDefault(x => x.Name.Equals("a"))
                ?.GetAttributeValue("href", string.Empty);

            return Regex.Match(hrefValue ?? string.Empty, 
                @"javascript:pf_pruefungprint\((.*)\);")
                .Groups[1]
                .Value;
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
