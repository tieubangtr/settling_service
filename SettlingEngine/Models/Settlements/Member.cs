using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Settlements
{
    [DbTable(Name = Const.DbTable.Member)]
    public class Member
    {
        [DbField(IsKey = true)]
        public string MemberId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string RecordStatus { get; set; } = string.Empty;
    }
}
