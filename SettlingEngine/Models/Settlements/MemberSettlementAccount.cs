using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Settlements
{
    [DbTable(Const.DbTable.MemberSettlementAccount)]
    public class MemberSettlementAccount
    {
        public string MemberId { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string AssetId { get; set; } = string.Empty;
        public string RecordStatus { get; set; } = string.Empty;

        public string? AssetType { get; set; }
        public string? BankAccount { get; set; }
    }
}
