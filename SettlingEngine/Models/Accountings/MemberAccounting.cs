using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Accountings
{
    [DbTable(Name = Const.DbTable.MemberAccounting, ViewName = Const.DbView.MemberAccounting)]
    public class MemberAccounting
    {
        [DbField(IsKey = true)]
        public string MemberAccountingId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string MemberId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string AssetId { get; set; } = string.Empty;

        public DateTime SettleDate { get; set; }
        public string MarketId { get; set; } = string.Empty;
        public string? CurrencyId { get; set; }
        public string? AccountingType { get; set; }
        public string? Debit_Credit { get; set; }
        public decimal AccountingValue { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }

        [DbField(IgnoreUpdate = true)]
        public string? CreateBy { get; set; }

        [DbField(IgnoreUpdate = true)]
        public DateTime CreateDate { get; set; }

        [DbField(IgnoreInsert = true)]
        public string? LastChangeBy { get; set; }

        [DbField(IgnoreInsert = true)]
        public DateTime LastChangeDate { get; set; }

        public string? ApproveBy { get; set; }
        public DateTime ApproveDate { get; set; }

        [DbField(IgnoreInsert = true, IgnoreUpdate = true)]
        public string CreateDateStr { get; set; } = string.Empty;
    }
}
