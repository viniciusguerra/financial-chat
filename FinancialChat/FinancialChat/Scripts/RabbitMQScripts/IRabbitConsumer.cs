using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialChat.Scripts.RabbitMQScripts
{
    public interface IRabbitConsumer
    {
        event RabbitConsumerDelegate OnMessageConsumed;
    }
}
