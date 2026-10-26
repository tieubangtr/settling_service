using DapperLib;
using SettlingEngine.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SettlingEngine.Models.Instruments
{
    [DbField(Name = Const.DbTable.Instrument)]
    public class Instrument
    {
        public string InstrumentId { get; set; } = string.Empty;
        public string AssetId { get; set; } = string.Empty;
        public string CurrencyId { get; set; } = string.Empty;
        public string? ProductClass { get; set; }
        public string? MarketId { get; set; }
        public string? Name { get; set; }
        public string RecordStatus { get; set; } = string.Empty;

        [DbField(Ignore = true)]
        public bool IsDerivative => ProductClass != null && ProductClass.Equals("FOS", StringComparison.OrdinalIgnoreCase);
    }
}
