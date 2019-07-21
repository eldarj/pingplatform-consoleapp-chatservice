using ChatMicroservice.SignalRServices.Interfaces;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ChatMicroservice.HostedServices
{
    public class ConsoleHostedService : IHostedService
    {
        private readonly ILogger logger;
        private readonly IApplicationLifetime appLifetime;

        private readonly IChatHubClientService chatHubClient;

        public ConsoleHostedService(
            IApplicationLifetime appLifetime,
            ILogger<ConsoleHostedService> logger,
            IChatHubClientService chatHubClient)
        {
            this.chatHubClient = chatHubClient;

            this.logger = logger;
            this.appLifetime = appLifetime;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            appLifetime.ApplicationStarted.Register(OnStarted);
            appLifetime.ApplicationStopping.Register(OnStopping);
            appLifetime.ApplicationStopped.Register(OnStopped);

            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            return Task.CompletedTask;
        }

        private void OnStarted()
        {
            logger.LogInformation("Starting ChatMicroservice (OnStarted)");

            try
            {
                // Connect to hubs
                chatHubClient.Connect();
            }
            catch (Exception e)
            {
                logger.LogInformation("ChatMicroservice couldn't be started (OnStarted)");
                return;
            }
        }

        private void OnStopping()
        {
            logger.LogInformation("Stopping ChatMicroservice (OnStopping)");
        }

        private void OnStopped()
        {
            logger.LogInformation("ChatMicroservice stopped (OnStopped)");
        }
    }
}
