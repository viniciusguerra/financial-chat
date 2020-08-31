using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using FinancialChat.Models;
using FinancialChat.Scripts;
using FinancialChat.Scripts.RabbitMQScripts;
using Microsoft.AspNetCore.Mvc;

namespace FinancialChat.Controllers
{
    public class MessageBoxController : Controller
    {
        private readonly RabbitProducer rabbitProducer;
        private readonly MessagePublisher messagePublisher;
        private readonly StockBot stockBot;

        bool redirect;

        public MessageBoxController(IRabbitProducer rabbitProducer, MessagePublisher messagePublisher, StockBot stockBot)
        {
            this.rabbitProducer = rabbitProducer as RabbitProducer;
            this.messagePublisher = messagePublisher;
            this.stockBot = stockBot;
        }

        private void SubmitMessage(string message)
        {
            rabbitProducer.PublishMessage(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Submit(MessageBoxViewModel viewModel)
        {
            redirect = false;

            SubmitMessage(viewModel.Message);

            messagePublisher.OnMessagePublished += OnMessagePublished;
            stockBot.OnStockRequestInvalid += OnMessageRejected;

            return await Redirect();
        }

        private void OnMessageRejected(string request)
        {
            redirect = true;

            messagePublisher.OnMessagePublished -= OnMessagePublished;
            stockBot.OnStockRequestInvalid -= OnMessageRejected;
        }

        private void OnMessagePublished(MessageModel message)
        {
            redirect = true;

            messagePublisher.OnMessagePublished -= OnMessagePublished;
            stockBot.OnStockRequestInvalid -= OnMessageRejected;
        }

        private async Task<RedirectToActionResult> Redirect()
        {
            while(!redirect)
            {
                await Task.Delay(100);
            }

            return RedirectToAction("Index", "Home");
        }
    }
}
