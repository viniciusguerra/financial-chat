using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using FinancialChat.Models;
using FinancialChat.Scripts;

namespace FinancialChat.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        StockBot stockBot;
        MessagePublisher messagePublisher;

        public HomeController(ILogger<HomeController> logger, StockBot stockBot, MessagePublisher messagePublisher)
        {
            _logger = logger;

            this.stockBot = stockBot;
            this.messagePublisher = messagePublisher;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
