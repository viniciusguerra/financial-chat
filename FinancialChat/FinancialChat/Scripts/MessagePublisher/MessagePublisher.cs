using FinancialChat.Data;
using FinancialChat.Models;
using FinancialChat.Scripts.RabbitMQScripts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialChat.Scripts
{
    public delegate void MessagePublishedDelegate(MessageModel message);

    public class MessagePublisher
    {
        private readonly FinancialChatContext context;
        private readonly RabbitConsumer consumer;

        public event MessagePublishedDelegate OnMessagePublished;        

        public MessagePublisher(FinancialChatContext context, IRabbitConsumer consumer)
        {
            this.context = context;
            this.consumer = consumer as RabbitConsumer;

            consumer.OnMessageConsumed += OnMessageConsumed;
        }

        private void OnMessageConsumed(MessageModel message, string routingKey)
        {
            if (routingKey != RabbitConstants.MESSAGE_QUEUE)
                return;

            Publish(message);
        }

        public async Task Publish(MessageModel message)
        {
            context.MessageModel.Add(message);

            await context.SaveChangesAsync();

            OnMessagePublished?.Invoke(message);
        }
    }
}
