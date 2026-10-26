using DapperLib;
using SettlingEngine.Constants;

namespace SettlingEngine.Models.Calendars
{
    [DbTable(Name = Const.DbTable.Calendar)]
    public sealed class Calendar
    {
        [DbField(IsKey = true)]
        public string CalendarId { get; set; } = string.Empty;

        public string? SaturdayType { get; set; }

        public string? SundayType { get; set; }

        [DbField(IsKey = true)]
        public string RecordStatus { get; set; } = string.Empty;

        [DbField(Ignore = true)]
        public List<CalendarDate> CalendarDates { get; set; } = [];

        [DbField(Ignore = true)]
        public HashSet<DateTime> CalendarDateSet { get; set; } = [];

    }
}
