using FinancialChat.Models;
using FinancialChat.Scripts.RabbitMQScripts;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace FinancialChatNUnitTestProject.MockClasses
{
    class RabbitProducerMock : IRabbitProducer
    {
        public event EventHandler<ProducerEventArgs> OnMessagePublished;

        public class ProducerEventArgs : EventArgs
        {
            public string Message;
        }

        public void PublishMessage(string message)
        {
            OnMessagePublished(this, new ProducerEventArgs() { Message = message });
        }

        public void PublishMessage(MessageModel message)
        {
            OnMessagePublished(this, new ProducerEventArgs() { Message = message.MessageBody });
        }
    }
}
