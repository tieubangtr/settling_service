using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Accounts
{
    [DbTable(Name = Const.DbTable.AccountBalance)]
    public class AccountBalance
    {
        public string AccountId { get; set; } = string.Empty;
        public string AssetId { get; set; } = string.Empty;
        public string? Status { get; set; }
        public decimal Balance { get; set; }
        public decimal HoldForTrade { get; set; }
        public decimal HoldForParty { get; set; }
        public decimal AvgPrice { get; set; }
        public decimal MatchSell0 { get; set; }
        public decimal MatchSell1 { get; set; }
        public decimal MatchSell2 { get; set; }
        public decimal MatchBuy0 { get; set; }
        public decimal MatchBuy1 { get; set; }
        public decimal MatchBuy2 { get; set; }
    }
}
