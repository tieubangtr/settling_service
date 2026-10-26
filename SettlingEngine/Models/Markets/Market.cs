using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Markets
{
    [DbTable(Name = Const.DbTable.Market)]
    public class Market
    {
        [DbField(IsKey = true)]
        public string MarketId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string RecordStatus { get; set; } = string.Empty;

        [DbField(Ignore = true)]
        public string DisplayName => $"Thị trường {MarketId}";

        public int DayOffset { get; set; }

        public string? CalendarId { get; set; }
    }

    public class MarketSettlement
    {
        public string MarketId { get; set; } = string.Empty;
        public int DayOffset { get; set; }
        public DateTime SettlementDate { get; set; }
        public DateTime TradeDate { get; set; }
    }
}
