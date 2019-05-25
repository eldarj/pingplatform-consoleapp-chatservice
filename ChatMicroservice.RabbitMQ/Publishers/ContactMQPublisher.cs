using ChatMicroservice.RabbitMQ.Publishers.Interfaces;
using ChatMicroservice.RabbitMQ.Utils;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Text;

namespace ChatMicroservice.RabbitMQ.Publishers
{
    public class ContactMQPublisher : IContactMQPublisher
    {
        private static ConnectionFactory connectionFactory;
        private static IConnection connection;
        private static IModel channel;

        private const string ExchangeType = "fanout";
        private const string ExchangeName = "CreateContact_FanoutExchange";
        private readonly string AccountQueueName = "AccountMicroservice_CreateContact_Queue";

        public ContactMQPublisher()
        {
            CreateConnection();
        }

        public void Close()
        {
            connection.Close();
        }

        public void CreateConnection()
        {
            connectionFactory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            connection = connectionFactory.CreateConnection();
            channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: ExchangeName,
                type: ExchangeType,
                durable: true,
                autoDelete: false,
                arguments: null);

            channel.QueueDeclare(queue: AccountQueueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null);

            channel.QueueBind(queue: AccountQueueName,
                exchange: ExchangeName,
                routingKey: "");
        }

        public void SendCreatedContact<T>(T contact)
        {
            var serializedContact = contact.Serialize();

            channel.BasicPublish(exchange: ExchangeName,
                routingKey: "",
                basicProperties: null,
                body: serializedContact);

            Console.WriteLine("RABBITMQ INFO: [Created New Contact] - Message sent to exchange-queue [{0}], data: {1}",
                ExchangeName,
                Encoding.Default.GetString(serializedContact));
        }
    }
}
