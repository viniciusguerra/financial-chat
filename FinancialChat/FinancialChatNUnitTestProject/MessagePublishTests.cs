using FinancialChat.Data;
using FinancialChat.Models;
using FinancialChat.Scripts;
using FinancialChatNUnitTestProject.MockClasses;
using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System.Linq;
using System.Threading.Tasks;

namespace FinancialChatNUnitTestProject
{
    public class MessagePublishTests
    {
        MessagePublisher messagePublisher;
        FinancialChatContext context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<FinancialChatContext>()
            .UseInMemoryDatabase(databaseName: "FinancialChatContext")
            .Options;

            context = new FinancialChatContext(options);

            RabbitConsumerMock consumer = new RabbitConsumerMock();

            messagePublisher = new MessagePublisher(context, consumer);
        }

        [Test]
        public async Task MessageIsPublishedToDatabaseTest()
        {
            MessageModel message = new MessageModel() { MessageBody = "Test Message" };

            await messagePublisher.Publish(message);

            Assert.IsTrue(context.MessageModel.Contains(message));
        }
    }
}