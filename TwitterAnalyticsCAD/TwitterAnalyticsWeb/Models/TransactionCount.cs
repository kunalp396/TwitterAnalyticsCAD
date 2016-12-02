using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace TwitterAnalyticsWeb.Models
{
    public class TransactionCount
    {
        public DateTime Timestamp { get; set; }
        public int Count { get; set; }
    }
}