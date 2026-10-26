using DapperLib;
using SettlingEngine.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlingEngine.Models.Accountings
{
    [DbTable(Name = Const.DbTable.AccountAccounting)]
    public class AccountAccounting
    {
        [DbField(IsKey = true)]
        public string AccountAccountingId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string AccountId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string AssetId { get; set; } = string.Empty;

        public DateTime SettleDate { get; set; }
        public string MemberId { get; set; } = string.Empty;
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
        public DateTime? ApproveDate { get; set; }
    }
}
