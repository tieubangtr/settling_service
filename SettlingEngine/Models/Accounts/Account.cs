using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Accounts
{
    [DbField(Name = Const.DbTable.Account)]
    public class Account
    {
        public string AccountId { get; set; } = string.Empty;
        public string? Name { get; set; }
        public string? AccountStatus { get; set; }
        public string RecordStatus { get; set; } = string.Empty;
    }
}
