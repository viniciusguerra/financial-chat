using FinancialChat.Scripts;
using FinancialChatNUnitTestProject.MockClasses;
using NUnit.Framework;

namespace FinancialChatNUnitTestProject
{
    public class StockBotTests
    {
        RabbitProducerMock producer;
        RabbitConsumerMock consumer;
        StockBot stockBot;

        [SetUp]
        public void Setup()
        {
            producer = new RabbitProducerMock();
            consumer = new RabbitConsumerMock();

            stockBot = new StockBot(consumer, producer);   
        }

        [Test]
        public void StockBotPublishesStockMessageTest()
        {
            consumer.ConsumeStockBotMessage();

            producer.OnMessagePublished += (e, a) =>
            {
                Assert.That(string.IsNullOrEmpty(a.Message));
            };
        }

        [Test]
        public void StockBotDoesntPublishCommonMessageTest()
        {
            consumer.ConsumeCommonMessage();

            stockBot.OnMessageRejected += (m) =>
            {
                Assert.Pass();
            };
        }

        [Test]
        public void StockBotRejectWrongRequestTest()
        {
            consumer.ConsumeWrongStockRequestMessage();

            stockBot.OnMessageRejected += (m) =>
            {
                Assert.Pass();
            };
        }
    }
}