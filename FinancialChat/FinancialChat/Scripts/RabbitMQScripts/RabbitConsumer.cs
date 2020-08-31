using FinancialChat.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace FinancialChat.Scripts.RabbitMQScripts
{
    public delegate void RabbitConsumerDelegate(MessageModel message, string routingKey);

    public class RabbitConsumer : RabbitHandler, IRabbitConsumer
    {
        public event RabbitConsumerDelegate OnMessageConsumed;

        public RabbitConsumer() : base()
        {
            var consumer = new EventingBasicConsumer(rabbitChannel);

            foreach (string queue in queues)
            {
                rabbitChannel.BasicConsume(
                    queue: queue,
                    autoAck: false, // set to false so that the message can be returned to queue in case of exceptions
                    consumer: consumer
                );
            }

            consumer.Received += OnReceived;            
        }

        private void OnReceived(object model, BasicDeliverEventArgs ea)
        {
            try
            {
                MessageModel message = DeserializeMessage(ea);

                Console.WriteLine("RabbitConsumer: Received message {0}", message);

                OnMessageConsumed?.Invoke(message, ea.RoutingKey);

                // acknowledge that message was received to remove it from queue
                rabbitChannel.BasicAck(ea.DeliveryTag, false);
            }
            catch (Exception e)
            {
                // do not acknowledge that message was received so that it can return to queue
                rabbitChannel.BasicNack(ea.DeliveryTag, false, true);

                Console.Error.WriteLine(e);
            }
        }

        private static MessageModel DeserializeMessage(BasicDeliverEventArgs ea)
        {
            var body = ea.Body.ToArray();

            MessageModel message;

            using (MemoryStream ms = new MemoryStream(body))
            {
                IFormatter br = new BinaryFormatter();

                message = (MessageModel)br.Deserialize(ms);
            }

            return message;
        }
    }
}
