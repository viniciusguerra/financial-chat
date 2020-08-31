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

        public MessageBoxController(IRabbitProducer rabbitProducer)
        {
            this.rabbitProducer = rabbitProducer as RabbitProducer;
        }

        private void SubmitMessage(string message)
        {
            rabbitProducer.PublishMessage(message);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Submit(MessageBoxViewModel viewModel)
        {
            SubmitMessage(viewModel.Message);

            return RedirectToAction("Index", "Home");            
        }
    }
}
