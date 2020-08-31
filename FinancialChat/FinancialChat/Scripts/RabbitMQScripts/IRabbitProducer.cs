using FinancialChat.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialChat.Scripts.RabbitMQScripts
{
    public interface IRabbitProducer
    {
        void PublishMessage(string message);
        void PublishMessage(MessageModel message);
    }
}
