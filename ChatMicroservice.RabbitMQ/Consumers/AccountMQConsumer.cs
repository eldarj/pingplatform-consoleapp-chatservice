using Api.DtoModels.Auth;
using ChatMicroservice.Data.Context;
using ChatMicroservice.Data.Models;
using ChatMicroservice.RabbitMQ.Consumers.Interfaces;
using ChatMicroservice.RabbitMQ.Utils;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.MessagePatterns;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChatMicroservice.RabbitMQ.Consumers
{
    public class AccountMQConsumer : IAccountMQConsumer
    {

        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;
        private static Subscription _subscription;
        private readonly MyDbContext dbContext;


        private readonly string ExchangeType = "fanout";
        private readonly string ExchangeName = "RegisterAccount_FanoutExchange";
        private readonly string QueueName = "ChatMicroservice_RegisterAccount_Queue";

        public AccountMQConsumer(MyDbContext dbContext)
        {
            this.dbContext = dbContext;
            CreateConnection();
        }

        public void CreateConnection()
        {
            Console.WriteLine("- Creating connection to RabbitMQ...");
            _factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();

            _model.ExchangeDeclare(exchange: ExchangeName,
                type: ExchangeType,
                durable: true,
                autoDelete: false,
                arguments: null);

            _model.QueueDeclare(queue: QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            _model.QueueBind(queue: QueueName,
                exchange: ExchangeName,
                routingKey: "");

            _model.BasicQos(0, 10, false);
            _subscription = new Subscription(_model, QueueName, false);
        }

        public void Close()
        {
            _connection.Close();
        }

        // We could call this upon instantiating the class (within the constructor), 
        // -- but we want to have the abillity to define the [T] param
        // -- so, we'll call ConsumeMessages right after we register this service, in Program.cs in this case
        public void ConsumeMessages()
        {
            Task.Run(async () =>
            {

                Console.WriteLine("Listening for Topic <payment.purchaseorder>");
                Console.WriteLine("------------------------------------------");

                // TODO: change this to an OnEvent listener, so we don't run it constantly - we'll trigger our consume from SignalR or something
                while (true)
                {
                    BasicDeliverEventArgs deliveryArguments = _subscription.Next();

                    var message = (AccountDto)deliveryArguments.Body.Deserialize(typeof(AccountDto));
                    var routingKey = deliveryArguments.RoutingKey;

                    Console.WriteLine("RABBITMQ INFO: [Account Registered] - Message received from exchange/queue [{0}/{1}], data: {2}",
                        ExchangeName,
                        QueueName,
                        Encoding.Default.GetString(deliveryArguments.Body));

                    if (await CreateNewUser(message))
                    {
                        _subscription.Ack(deliveryArguments);
                    }

                }
            });

        }

        // CREATE A DATA SERVICE THAT WILL HAVE INJECTED RABBITMQ CONSUMER
        // AFTER INJECTION; DON't RUN THE CONSUMER, BUT WAIT FOR SIGNALR MESSAGES
        // SO... when we get a signalr message from the API, we then go into the rabbitmq and get it
        // otherwise, we'll geet the messages on each microservice startup aswell, so we stay up-to-date (regarding data duplication)
        // Also, check that we always have different queuest for duplicating data, for each microservice that we have (eg. DataSpace and Chat microserv.)
        // We use the same fanout tho - and also should define defeiniton and creation of that fanout and each queue, in each microservice.

        public async Task<bool> CreateNewUser(AccountDto accountDto)
        {
            var account = dbContext.Accounts.Where(a => a.PhoneNumber == accountDto.PhoneNumber).SingleOrDefault();
            if (account != null) return false;

            account = new Account
            {
                Id = accountDto.Id,
                PhoneNumber = accountDto.PhoneNumber,
                Firstname = accountDto.Firstname,
                Lastname = accountDto.Lastname,
                AvatarImageUrl = accountDto.AvatarImageUrl
            };

            dbContext.Accounts.Add(account);

            await dbContext.SaveChangesAsync();

            return true;
        }
    }
}
