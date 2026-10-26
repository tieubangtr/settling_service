using SettlingEngine.Models.Settlements;

namespace SettlingEngine.Constants
{
    public class Const
    {
        public static class DbTable
        {
            public const string Member = "MEMBER";
            public const string MemberSettlement = "MEMBER_SETTLEMENT";
            public const string MemberSettlementAccount = "MEMBER_SETTLEMENTACCOUNT";

            public const string MemberAccounting = "MEMBER_ACCOUNTING";
            public const string AccountAccounting = "ACCOUNT_ACCOUNTING";

            public const string OrderMatch = "ORDER_MATCH";

            public const string Account = "ACCOUNT";
            public const string AccountBalance = "ACCOUNT_BALANCE";

            public const string Market = "MARKET";

            public const string Instrument = "INSTRUMENT";
            public const string AssetDefinition = "ASSET_DEFINITION";

            public const string Calendar = "CALENDAR";
            public const string CalendarDate = "CALENDAR_DATE";
        }

        public static class DbView
        {
            public const string ORDERMATCH_SETTLE = "VW_ORDER_MATCH_SETTLE";
            public const string MemberSettlement = "VW_MEMBER_SETTLEMENT";
            public const string MemberAccounting = "VW_MEMBER_ACCOUNTING";
        }

        public static class Settlement_Status
        {
            public const string Unsettled = "U";
            public const string Settled = "S";
        }

        public static class MemberSettlement_IdentityPrefix
        {
            public const string Settlement = "S";
        }

        public static class YN
        {
            public const string Yes = "Y";
            public const string No = "No";
        }

        public static class RecordStatus
        {
            public const string Active = "A";
        }

        public static class Market_DayOffset
        {
            public const int T0 = 0;
            public const int T1 = 1;
            public const int T2 = 2;
        }

        public static class Debit_Credit
        {
            public const string Debit = "D";
            public const string Credit = "C";
        }

        public static class MemberSettlementAccount_Default
        {
            public const string CCPMember = "CCP";
            public const string Market_All = "ALL";
        }

        public static class CalendarDate_DataType
        {
            public const string Y = "Y";
        }
    }
}
