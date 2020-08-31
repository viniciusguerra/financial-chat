using FinancialChat.Models;
using FinancialChat.Scripts.RabbitMQScripts;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Text;

namespace FinancialChatNUnitTestProject.MockClasses
{
    class RabbitConsumerMock : IRabbitConsumer
    {
        public event RabbitConsumerDelegate OnMessageConsumed;

        public void ConsumeStockBotMessage()
        {
            MessageModel message = new MessageModel();

            message.MessageBody = "/stock=aapl.us";

            OnMessageConsumed.Invoke(message, RabbitConstants.STOCK_BOT_QUEUE);
        }

        public void ConsumeCommonMessage()
        {
            MessageModel message = new MessageModel();

            message.MessageBody = "Common Message";

            OnMessageConsumed.Invoke(message, RabbitConstants.MESSAGE_QUEUE);
        }

        public void ConsumeWrongStockRequestMessage()
        {
            MessageModel message = new MessageModel();

            message.MessageBody = "Wrong Id";

            OnMessageConsumed.Invoke(message, RabbitConstants.MESSAGE_QUEUE);
        }
    }
}
