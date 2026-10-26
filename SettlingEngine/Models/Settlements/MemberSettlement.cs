using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Settlements
{
    [DbTable(Const.DbTable.MemberSettlement, ViewName = Const.DbView.MemberSettlement)]
    public class MemberSettlement
    {
        [DbField(IsKey = true)]
        public string SettlementId { get; set; } = string.Empty;
        public string MemberId { get; set; } = string.Empty;
        public string MarketId { get; set; } = string.Empty;
        public string AssetId { get; set; } = string.Empty;

        public string? Debit_Credit { get; set; }

        [DbField(IgnoreInsert = true, IgnoreUpdate = true)]
        public string? Debit_CreditText { get; set; }

        public string? SettlementAccountCCP { get; set; }
        public string? SettlementAccountMember { get; set; }
        public string MemberAccountingId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string? Description { get; set; }

        public string? Status { get; set; }
        public DateTime SettleDate { get; set; }

        [DbField(IgnoreInsert = true, IgnoreUpdate = true)]
        public string? StatusText { get; set; }

        [DbField(IgnoreInsert = true, IgnoreUpdate = true)]
        public string? DebitAccount { get; set; }

        [DbField(IgnoreInsert = true, IgnoreUpdate = true)]
        public string? CreditAccount { get; set; }

        [DbField(IgnoreInsert = true, IgnoreUpdate = true)]
        public string? MemberName { get; set; }

        [DbField(IgnoreInsert = true, IgnoreUpdate = true)]
        public string? DateString { get; set; }

        [DbField(IgnoreUpdate = true)]
        public string? CreateBy { get; set; }

        [DbField(IgnoreUpdate = true)]
        public DateTime CreateDate { get; set; }

        [DbField(IgnoreInsert = true)]
        public string? LastChangeBy { get; set; }
        
        [DbField(IgnoreInsert = true)]
        public DateTime LastChangeDate { get; set; }

        
    }
}
