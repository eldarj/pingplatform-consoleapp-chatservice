using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using ChatMicroservice.Data.Context;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using ChatMicroservice.RabbitMQ.Consumers.Interfaces;
using ChatMicroservice.RabbitMQ.Consumers;
using ChatMicroservice.Data.Services.Interfaces;
using ChatMicroservice.Data.Services;
using ChatMicroservice.RabbitMQ.Publishers;
using ChatMicroservice.RabbitMQ.Publishers.Interfaces;
using Ping.Commons.Settings;
using ChatMicroservice.HostedServices;
using ChatMicroservice.SignalRServices.Interfaces;
using ChatMicroservice.SignalRServices;

namespace ChatMicroservice
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            Console.WriteLine("::ChatMicroservice::");

            var host = new HostBuilder()
                .ConfigureHostConfiguration(configHost =>
                {
                    configHost.SetBasePath(Directory.GetCurrentDirectory());
                    configHost.AddJsonFile("hostsettings.json", optional: true);
                    configHost.AddEnvironmentVariables(prefix: "PREFIX_");
                    configHost.AddCommandLine(args);
                })
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    configApp.AddJsonFile("appsettings.json", optional: false);
                    configApp.AddEnvironmentVariables(prefix: "PREFIX_");
                    configApp.AddCommandLine(args);
                })
                .ConfigureServices((hostContext, services) =>
                {
                    //services.AddDbContextPool<MyDbContext>(options =>
                    //    options.UseMySql(hostContext.Configuration.GetConnectionString("MysqlAccountMicroservice"), b =>
                    //        b.MigrationsAssembly("AccountMicroservice.Data"))
                    //);
                    services.AddDbContext<MyDbContext>();

                    // Jwt authentication// configure strongly typed settings objects
                    services.Configure<SecuritySettings>(hostContext.Configuration.GetSection("SecuritySettings"));
                    services.Configure<GatewayBaseSettings>(hostContext.Configuration.GetSection("GatewayBaseSettings"));

                    services.AddHostedService<ConsoleHostedService>();
                    services.AddHostedService<AccountMQConsumer>();

                    services.AddScoped<IEmojiService, EmojiService>();
                    services.AddScoped<IAccountService, AccountService>();
                    services.AddScoped<IContactService, ContactService>();
                    services.AddScoped<IMessagingService, MessagingService>();

                    services.AddScoped<IChatHubClientService, ChatHubClientService>();

                    services.AddScoped<IContactMQPublisher, ContactMQPublisher>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                    configLogging.AddFilter("Microsoft.EntityFrameworkCore.Database.Command", LogLevel.Warning);
                })
                .UseConsoleLifetime()
                .Build();

            await host.RunAsync();
        }
    }
}
