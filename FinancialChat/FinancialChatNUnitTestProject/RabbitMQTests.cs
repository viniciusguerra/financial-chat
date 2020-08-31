using FinancialChat.Scripts.RabbitMQScripts;
using NUnit.Framework;

namespace FinancialChatNUnitTestProject
{
    public class RabbitMQTests
    {
        RabbitConsumer rabbitConsumer;
        RabbitProducer rabbitProducer;

        [SetUp]
        public void Setup()
        {
            rabbitConsumer = new RabbitConsumer();
            rabbitProducer = new RabbitProducer(null);
        }

        [Test]
        // RabbitMQ must be running for this test to pass
        public void ProduceConsumeTest()
        {
            string testMessage = "Test Message";

            rabbitProducer.PublishMessage(testMessage);

            rabbitConsumer.OnMessageConsumed += (m, k) =>
            {
                Assert.AreEqual(m.MessageBody, testMessage);
            };
        }
    }
}