using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Instruments
{
    [DbTable(Name = Const.DbTable.AssetDefinition)]
    public class AssetDefinition
    {
        [DbField(IsKey = true)]
        public string AssetId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string RecordStatus { get; set; } = string.Empty;

        public string? AssetType { get; set; }
        public string? Name { get; set; }
        public string? Description { get; set; }

    }
}
