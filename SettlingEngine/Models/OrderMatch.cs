using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models
{
    [DbTable(Name = Const.DbTable.OrderMatch, ViewName = Const.DbView.ORDERMATCH_SETTLE)]
    public class OrderMatch
    {
        [DbField(IsKey = true)]
        public string OrderId { get; set; } = string.Empty;

        public string? InstrumentId { get; set; }

        public string? AccountId { get; set; }

        public string? Side { get; set; }

        public decimal MatchPrice { get; set; }

        public decimal MatchQtty { get; set; }

        public decimal MatchValue { get; set; }

        public DateTime? TimeExecute { get; set; }

        public DateTime TradeDate { get; set; }

        public string? Is_Payment_Processed { get; set; }

        [DbField(Ignore = true)]
        public string MarketSettleKey { get; set; } = string.Empty;
    }
}
