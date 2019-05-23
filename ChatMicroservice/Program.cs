using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using ChatMicroservice.Data.Context;

using ChatMicroservice.SignalR.ClientServices;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.FileProviders;
using ChatMicroservice.RabbitMQ.Consumers.Interfaces;
using ChatMicroservice.RabbitMQ.Consumers;

namespace ChatMicroservice
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
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

                    services.AddHostedService<SignalRClientService>();

                    IAccountMQConsumer mqConsumer = new AccountMQConsumer();
                    mqConsumer.ConsumeMessages();

                    //services.AddSingleton<IAccountMQConsumer, AccountMQConsumer>();
                })
                .ConfigureLogging((hostContext, configLogging) =>
                {
                    configLogging.AddConsole();
                    configLogging.AddDebug();
                })
                .UseConsoleLifetime()
                .Build();
            Console.WriteLine("::ChatMicroservice::");



            await host.RunAsync();
        }
    }
}
