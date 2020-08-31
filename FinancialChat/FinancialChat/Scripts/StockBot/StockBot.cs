using FinancialChat.Models;
using FinancialChat.Scripts.RabbitMQScripts;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace FinancialChat.Scripts
{
    public delegate void StockBotRejectedMessageDelegate(string request);

    public class StockBot
    {
        private const string ERROR_RESPONSE = "N/D";

        private const string STOCK_API_BASE_ADDRESS = @"https://stooq.com/q/l/";
        private const string MESSAGE_FORMAT = "{0} quote is {1} per share";
        private const string STOCK_BOT_ID = "Stock Bot";
        public static string STOCK_PREFIX = @"/stock";

        private readonly RabbitConsumer consumer;
        private readonly RabbitProducer producer;
        private readonly HttpClient httpClient;

        public event StockBotRejectedMessageDelegate OnMessageRejected;
        public event StockBotRejectedMessageDelegate OnStockRequestInvalid;

        public StockBot(IRabbitConsumer consumer, IRabbitProducer producer)
        {
            this.consumer = consumer as RabbitConsumer;
            this.producer = producer as RabbitProducer;

            consumer.OnMessageConsumed += OnMessageConsumed;

            this.httpClient = new HttpClient() { BaseAddress = new Uri(STOCK_API_BASE_ADDRESS) };
        }

        private async void OnMessageConsumed(MessageModel message, string routingKey)
        {
            if (routingKey != RabbitConstants.STOCK_BOT_QUEUE)
            {
                OnMessageRejected?.Invoke(message.MessageBody);

                return;
            }

            string stockId = message.MessageBody.Replace(STOCK_PREFIX + "=", "");

            var query = new Dictionary<string, string>
            {
                {"s", stockId},
                {"f", "sd2t2ohlcv"},
                {"e", "csv"}
            };

            var urlAct = QueryHelpers.AddQueryString(
                $"", query
            );

            var response = await httpClient.GetAsync(urlAct);

            if(response.IsSuccessStatusCode)
            {
                StreamReader sr = new StreamReader(await response.Content.ReadAsStreamAsync());
                string responseContent = sr.ReadToEnd();
                sr.Close();

                string[] responseContentSplit = responseContent.Split(',');

                string stockName = responseContentSplit[0];

                if(responseContentSplit[1] == ERROR_RESPONSE)
                {
                    StockRequestInvalid(stockId);

                    return;
                }

                string stockValue = string.Format(@"${0:0.00}", decimal.Parse(responseContentSplit[6]));

                message.MessageBody = string.Format(MESSAGE_FORMAT, stockName, stockValue);
                message.OwnerName = STOCK_BOT_ID;

                producer.PublishMessage(message);
            }     
            else
            {
                StockRequestInvalid(stockId);
            }
        }

        private void StockRequestInvalid(string requestedStock)
        {
            Debug.Write(string.Format("StockBot: Could not load data for stockId={0}", requestedStock));

            OnStockRequestInvalid?.Invoke(requestedStock);
        }
    }
}
