using FinancialChat.Data;
using FinancialChat.Models;
using FinancialChat.Scripts.RabbitMQScripts;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialChat.Scripts
{
    public delegate void MessagePublishedDelegate(MessageModel message);

    public class MessagePublisher
    {
        private readonly RabbitConsumer consumer;
        private FinancialChatContext context;

        public event MessagePublishedDelegate OnMessagePublished;

        IConfiguration configuration;

        public MessagePublisher(IRabbitConsumer consumer, IConfiguration configuration)
        {
            this.configuration = configuration;

            this.consumer = consumer as RabbitConsumer;

            consumer.OnMessageConsumed += OnMessageConsumed;
        }

        private async void OnMessageConsumed(MessageModel message, string routingKey)
        {
            if (routingKey != RabbitConstants.MESSAGE_QUEUE)
                return;

            await Publish(message);
        }

        public async Task Publish(MessageModel message)
        {
            BuildEFContext();

            await context.MessageModel.AddAsync(message);

            await context.SaveChangesAsync();

            OnMessagePublished?.Invoke(message);
        }

        private void BuildEFContext()
        {
            DbContextOptionsBuilder<FinancialChatContext> builder = new DbContextOptionsBuilder<FinancialChatContext>();

            builder.UseSqlServer(configuration.GetConnectionString("FinancialChatContext"));

            DbContextOptions<FinancialChatContext> options = builder.Options;

            context = new FinancialChatContext(options);
        }
    }
}
