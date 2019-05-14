using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DaHo.SephirWatcher.Models;
using DaHo.SephirWatcher.Utilities;
using DaHo.SephirWatcher.Web.Data;
using DaHo.SephirWatcher.Web.Helper;
using DaHo.SephirWatcher.Web.Interfaces;
using DaHo.SephirWatcher.Web.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Server.Kestrel.Core.Internal.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DaHo.SephirWatcher.Web.Services
{
    public class SephirMarkWatcherService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private readonly ILogger<SephirMarkWatcherService> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IStringCipher _stringCipher;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        private Task _executingTask;

        public SephirMarkWatcherService(IServiceScopeFactory serviceScopeFactory, ILogger<SephirMarkWatcherService> logger, IEmailSender emailSender, IStringCipher stringCipher)
        {
            _serviceScopeFactory = serviceScopeFactory;
            _logger = logger;
            _emailSender = emailSender;
            _stringCipher = stringCipher;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This will cause the loop to stop if the service is stopped
            while (!stoppingToken.IsCancellationRequested)
            {
                _logger.LogInformation($"Starting the watcher service at {DateTime.Now}");
                try
                {
                    using (var scope = _serviceScopeFactory.CreateScope())
                    {
                        var context = scope.ServiceProvider.GetRequiredService<SephirContext>();

                        var allSephirLogins = await context
                            .SephirLogins
                            .Include(x => x.IdentityUser)
                            .ToListAsync(stoppingToken);
                        _logger.LogInformation($"Watching {allSephirLogins.Count} sephir accounts");

                        var tasks = new List<Task>();
                        foreach (var sephirLogin in allSephirLogins)
                        {
                            tasks.Add(ExecuteActionAsync(sephirLogin, scope, stoppingToken));
                        }

                        Task.WaitAll(tasks.ToArray(), stoppingToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError(e, "An exception occured while running the watcher-service");
                }
                finally
                {
                    // Wait 10 minutes before running again.
                    await Task.Delay(TimeSpan.FromMinutes(10), stoppingToken);
                }

            }
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // Store the task we're executing
            _executingTask = ExecuteAsync(_stoppingCts.Token);

            // If the task is completed then return it,
            // this will bubble cancellation and failure to the caller
            if (_executingTask.IsCompleted)
            {
                return _executingTask;
            }

            // Otherwise it's running
            return Task.CompletedTask;
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            // Stop called without start
            if (_executingTask == null)
            {
                return;
            }

            try
            {
                // Signal cancellation to the executing method
                _stoppingCts.Cancel();
            }
            finally
            {
                // Wait until the task completes or the stop token triggers
                await Task.WhenAny(_executingTask, Task.Delay(Timeout.Infinite,
                    cancellationToken));
            }
        }

        public void Dispose()
        {
            _stoppingCts.Cancel();
        }

        private async Task ExecuteActionAsync(SephirLogin login, IServiceScope scope, CancellationToken cancellationToken)
        {
            var sephirWatcher = new SephirWatcher(new SephirAccount
            {
                AccountEmail = login.EmailAdress,
                AccountPassword = _stringCipher.Decrypt(login.EncryptedPassword)
            });

            if (!await sephirWatcher.AreCredentialsValid())
            {
                await HandleInvalidSephirCredentials(login);
            }

            var context = scope.ServiceProvider.GetRequiredService<SephirContext>();

            var existingTests = await GetSavedTestsForLogin(login, cancellationToken, context);

            var testsInSephir = await GetTestsInSephirForLogin(login, sephirWatcher);
            var newTests = testsInSephir.Except(existingTests, new SephirTestWithoutIdsComparer()).ToList();

            if (newTests.Any())
            {
                await HandleNewTestsInSephir(login, cancellationToken, context, newTests);
            }
        }

        private async Task HandleNewTestsInSephir(SephirLogin login, CancellationToken cancellationToken,
            SephirContext context, List<SephirTest> newTests)
        {
            await context.SephirTests.AddRangeAsync(newTests, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            string emailText = $"Du hast in den folgenden Fächern eine neue Note im Sephir:<br>{HtmlUtilities.CreateUnorderedList(newTests)}";

            await _emailSender.SendEmailAsync(login.IdentityUser.Email, "Neue Note im Sephir",
                emailText);
        }

        private async Task HandleInvalidSephirCredentials(SephirLogin login)
        {
            await _emailSender.SendEmailAsync(login.IdentityUser.Email, "Ungültige Account-Daten",
                "Der SephirWatcher kann sich nicht mehr mit deinem Sephir-Account einloggen. Bitte passe deine Daten auf unserer Webseite an.");
        }

        private async Task<List<SephirTest>> GetSavedTestsForLogin(SephirLogin login, CancellationToken cancellationToken,
            SephirContext context)
        {
            var existingTests = await context
                .SephirTests
                .Where(x => x.SephirLogin.Id == login.Id)
                .ToListAsync(cancellationToken);
            existingTests.ForEach(x => x.Mark = _stringCipher.Decrypt(x.EncryptedMark));

            return existingTests;
        }

        private async Task<IEnumerable<SephirTest>> GetTestsInSephirForLogin(SephirLogin login, SephirWatcher watcher)
        {
            return (await watcher.GetSephirExamsForAllClasses())
                .Where(x => x.Mark.HasValue)
                .Select(x => new SephirTest
                {
                    ExamDate = x.ExamDate,
                    ExamTitle = x.ExamTitle,
                    EncryptedMark = _stringCipher.Encrypt(x.Mark.ToString()),
                    Mark = x.Mark.ToString(),
                    MarkType = x.MarkType,
                    MarkWeighting = x.MarkWeighting,
                    SchoolSubject = x.SchoolSubject,
                    SephirLoginId = login.Id
                });
        }
    }
}