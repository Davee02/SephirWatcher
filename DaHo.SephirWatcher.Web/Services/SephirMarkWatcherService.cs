using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DaHo.SephirWatcher.Models;
using DaHo.SephirWatcher.Web.Data;
using DaHo.SephirWatcher.Web.Helper;
using DaHo.SephirWatcher.Web.Interfaces;
using DaHo.SephirWatcher.Web.Models;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace DaHo.SephirWatcher.Web.Services
{
    public class SephirMarkWatcherService : IHostedService, IDisposable
    {
        private readonly IServiceScopeFactory _serviceScopeFactory;

        private Task _executingTask;
        private readonly CancellationTokenSource _stoppingCts = new CancellationTokenSource();

        public SephirMarkWatcherService(IServiceScopeFactory serviceScopeFactory)
        {
            _serviceScopeFactory = serviceScopeFactory;
        }

        protected async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            // This will cause the loop to stop if the service is stopped
            while (!stoppingToken.IsCancellationRequested)
            {
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var context = scope.ServiceProvider.GetRequiredService<SephirContext>();

                    var allSephirLogins = await context
                        .SephirLogins
                        .Include(x => x.IdentityUser)
                        .ToListAsync(stoppingToken);
                    var tasks = new List<Task>();

                    foreach (var sephirLogin in allSephirLogins)
                    {
                        tasks.Add(ExecuteActionAsync(sephirLogin, scope, stoppingToken));
                    }

                    Task.WaitAll(tasks.ToArray(), stoppingToken);

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
            var context = scope.ServiceProvider.GetRequiredService<SephirContext>();
            var cipher = scope.ServiceProvider.GetRequiredService<IStringCipher>();
            var emailSender = scope.ServiceProvider.GetRequiredService<IEmailSender>();

            var existingTests = await GetSavedTestsForLogin(login, cancellationToken, context);
            var testsInSephir = await GetTestsInSephirForLogin(login, cipher);

            var newTests = testsInSephir.Except(existingTests, new SephirTestWithoutIdsComparer()).ToList();

            if (newTests.Any())
            {
                await HandleNewTestsInSephir(login, cancellationToken, context, newTests, emailSender);
            }
        }

        private static async Task HandleNewTestsInSephir(SephirLogin login, CancellationToken cancellationToken,
            SephirContext context, List<SephirTest> newTests, IEmailSender emailSender)
        {
            await context.SephirTests.AddRangeAsync(newTests, cancellationToken);
            await context.SaveChangesAsync(cancellationToken);

            await emailSender.SendEmailAsync(login.IdentityUser.Email, "Neue Note im Sephir",
                "Du hast eine neue Note im Sephir!");
        }

        private static async Task<List<SephirTest>> GetSavedTestsForLogin(SephirLogin login, CancellationToken cancellationToken,
            SephirContext context)
        {
            var existingTests = await context
                .SephirTests
                .Where(x => x.SephirLogin.Id == login.Id)
                .ToListAsync(cancellationToken);
            return existingTests;
        }

        private static async Task<IEnumerable<SephirTest>> GetTestsInSephirForLogin(SephirLogin login, IStringCipher cipher)
        {
            var sephirWatcher = new SephirWatcher(new SephirAccount
            {
                AccountEmail = login.EmailAdress,
                AccountPassword = cipher.Decrypt(login.EncryptedPassword)
            });

            return (await sephirWatcher.GetSephirExamsForAllClasses())
                .Where(x => x.Mark.HasValue)
                .Select(x => new SephirTest
                {
                    ExamDate = x.ExamDate,
                    ExamState = x.ExamState,
                    ExamTitle = x.ExamTitle,
                    Mark = x.Mark,
                    MarkType = x.MarkType,
                    MarkWeighting = x.MarkWeighting,
                    SchoolSubject = x.SchoolSubject,
                    SephirLoginId = login.Id
                });
        }
    }
}