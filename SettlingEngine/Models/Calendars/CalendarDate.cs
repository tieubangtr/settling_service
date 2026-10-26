using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Calendars
{
    [DbTable(Name = Const.DbTable.CalendarDate)]
    public sealed class CalendarDate
    {
        [DbField(IsKey = true)]
        public string CalendarId { get; set; } = string.Empty;

        [DbField(IsKey = true)]
        public string DateValue { get; set; } = string.Empty;

        public string? DataType { get; set; }

        public string? Description { get; set; }

        [DbField(IsKey = true)]
        public string RecordStatus { get; set; } = string.Empty;
    }
}
