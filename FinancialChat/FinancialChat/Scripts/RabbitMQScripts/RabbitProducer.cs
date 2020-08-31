using FinancialChat.Models;
using Microsoft.AspNetCore.Http;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;

namespace FinancialChat.Scripts.RabbitMQScripts
{
    public class RabbitProducer : RabbitHandler, IRabbitProducer
    {
        IHttpContextAccessor httpContextAccessor;

        public RabbitProducer(IHttpContextAccessor httpContextAccessor) : base()
        {
            this.httpContextAccessor = httpContextAccessor;
        }

        private void ParseMessage(string message, out byte[] body, out string queue)
        {
            MessageModel messageInstance = new MessageModel();

            messageInstance.MessageBody = message;
            messageInstance.Timestamp = DateTime.Now;

            ParseMessage(messageInstance, out body, out queue);
        }

        private void ParseMessage(MessageModel message, out byte[] body, out string queue)
        {
            if (message.MessageBody.StartsWith(StockBot.STOCK_PREFIX))
            {
                queue = RabbitConstants.STOCK_BOT_QUEUE;
            }
            else
            {
                queue = RabbitConstants.MESSAGE_QUEUE;

                if(string.IsNullOrEmpty(message.OwnerName) && httpContextAccessor != null)
                    message.OwnerName = httpContextAccessor.HttpContext.User.Identity.Name;
            }

            body = SerializeMessageToBinary(message);
        }

        private static byte[] SerializeMessageToBinary(MessageModel message)
        {
            byte[] body;
            BinaryFormatter bf = new BinaryFormatter();

            using (MemoryStream ms = new MemoryStream())
            {
                bf.Serialize(ms, message);
                body = ms.ToArray();
            }

            return body;
        }

        private void PublishMessage(byte[] body, string queue)
        {
            rabbitChannel.BasicPublish(exchange: "",
                                    routingKey: queue,
                                    basicProperties: null,
                                    body: body);            
        }

        public void PublishMessage(string message)
        {
            byte[] body;
            string queue;

            ParseMessage(message, out body, out queue);

            PublishMessage(body, queue);

            Console.WriteLine("MessageBoxController: Published {0} to {1} queue", message, queue);
        }

        public void PublishMessage(MessageModel message)
        {
            byte[] body;
            string queue;

            ParseMessage(message, out body, out queue);

            PublishMessage(body, queue);

            Console.WriteLine("MessageBoxController: Published {0} to {1} queue", message, queue);
        }
    }
}
