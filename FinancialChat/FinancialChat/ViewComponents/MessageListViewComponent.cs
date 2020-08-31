using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using FinancialChat.Data;
using FinancialChat.Models;

namespace FinancialChat.ViewComponents
{
    public class MessageListViewComponent : ViewComponent
    {
        private const int MAX_MESSAGE_COUNT = 50;

        private readonly FinancialChatContext context;

        public MessageListViewComponent(FinancialChatContext context)
        {
            this.context = context;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            List<MessageModel> messageList = await context
                .MessageModel.OrderBy(x => x.Timestamp)                                 // order messages by Timestamp
                .Skip(Math.Max(0, context.MessageModel.Count() - MAX_MESSAGE_COUNT))   // get only the 50 latest messages
                .AsNoTracking()
                .ToListAsync();

            return View(messageList);
        }
    }
}
