using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using FinancialChat.Models;

namespace FinancialChat.Data
{
    public class FinancialChatContext : DbContext
    {
        public FinancialChatContext (DbContextOptions<FinancialChatContext> options)
            : base(options)
        {
        }

        public DbSet<MessageModel> MessageModel { get; set; }
    }
}
