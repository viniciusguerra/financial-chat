using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialChat.Scripts.RabbitMQScripts
{
    public abstract class RabbitHandler
    {
        protected IConnection rabbitConnection;
        protected IModel rabbitChannel;
        protected string[] queues;

        public RabbitHandler()
        {
            queues = new string[] { RabbitConstants.MESSAGE_QUEUE, RabbitConstants.STOCK_BOT_QUEUE };

            var factory = new ConnectionFactory() { HostName = "localhost" };

            rabbitConnection = factory.CreateConnection();
            rabbitChannel = rabbitConnection.CreateModel();

            foreach (string queue in queues)
            {
                rabbitChannel.QueueDeclare(queue: queue,
                                        durable: false,
                                        exclusive: false,
                                        autoDelete: false,
                                        arguments: null);
            }
        }
    }
}
