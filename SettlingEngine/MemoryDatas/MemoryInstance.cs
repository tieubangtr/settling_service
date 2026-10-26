using Microsoft.Extensions.DependencyInjection;
using SettlingEngine.Constants;
using SettlingEngine.DataAccesses;
using SettlingEngine.Models.Accounts;
using SettlingEngine.Models.Calendars;
using SettlingEngine.Models.Instruments;
using SettlingEngine.Models.Markets;
using SettlingEngine.Models.Settlements;

namespace SettlingEngine.MemoryDatas
{
    internal interface IMemoryInstance
    {
        Task Initialize();
        Task Reload();

        IReadOnlyList<Account> GetAccounts();
        IReadOnlyList<Instrument> GetInstruments();
        IReadOnlyList<Market> GetMarkets();
        IReadOnlyList<Member> GetMembers();
        IReadOnlyList<MemberSettlementAccount> GetMemberSettlementAccounts();
        IReadOnlyList<AssetDefinition> GetAssets();

        Instrument GetInstrumentById(string id);
        Market GetMarketById(string id);
        Member GetMemberById(string id);
        AssetDefinition GetAssetById(string id);

        string GetMemberSettlementAccount(string memberId, string marketId);

        DateTime GetNextWorkingDate(string? calendarId, DateTime date, int step);
        DateTime GetNextWorkingDate(string marketId, DateTime currentDate);
        DateTime GetCurrentWorkingDate();
        DateTime GetPreviousWorkingDate(string marketId, DateTime currentDate);
    }

    internal class MemorySnapshot
    {
        public List<Account> Accounts { get; init; } = [];
        public List<Instrument> Instruments { get; init; } = [];
        public List<Market> Markets { get; init; } = [];
        public List<Member> Members { get; init; } = [];
        public List<MemberSettlementAccount> SettlementAccounts { get; init; } = [];
        public List<AssetDefinition> Assets { get; init; } = [];
        public List<Calendar> Calendars { get; init; } = [];

        public Dictionary<string, Account> AccountMapById { get; init; } = [];
        public Dictionary<string, Instrument> InstrumentMapById { get; init; } = [];
        public Dictionary<string, Market> MarketMapById { get; init; } = [];
        public Dictionary<string, Member> MemberMapById { get; init; } = [];
        public Dictionary<string, AssetDefinition> AssetMapById { get; init; } = [];
        public Dictionary<string, Calendar> CalendarMapById { get; init; } = [];
    }

    internal class MemoryInstance : IMemoryInstance
    {
        private readonly IDataAccess _dataAccess;

        private MemorySnapshot _snapshot = new();

        public MemoryInstance(IServiceScopeFactory serviceScopeFactory)
        {
            var serviceScope = serviceScopeFactory.CreateScope();
            _dataAccess = serviceScope.ServiceProvider.GetRequiredService<IDataAccess>();
        }

        public IReadOnlyList<Account> GetAccounts() => _snapshot.Accounts;
        public IReadOnlyList<Instrument> GetInstruments() => _snapshot.Instruments;
        public IReadOnlyList<Market> GetMarkets() => _snapshot.Markets;
        public IReadOnlyList<Member> GetMembers() => _snapshot.Members;
        public IReadOnlyList<MemberSettlementAccount> GetMemberSettlementAccounts() => _snapshot.SettlementAccounts;
        public IReadOnlyList<AssetDefinition> GetAssets() => _snapshot.Assets;

        public Account GetAccountById(string id) => _snapshot.AccountMapById.TryGetValue(id, out var data) ? data : new();
        public Instrument GetInstrumentById(string id) => _snapshot.InstrumentMapById.TryGetValue(id, out var data) ? data : new();
        public Market GetMarketById(string id) => _snapshot.MarketMapById.TryGetValue(id, out var data) ? data : new();
        public Member GetMemberById(string id) => _snapshot.MemberMapById.TryGetValue(id, out var data) ? data : new();
        public AssetDefinition GetAssetById(string id) => _snapshot.AssetMapById.TryGetValue(id, out var data) ? data : new();
        public string GetMemberSettlementAccount(string memberId, string marketId)
        {
           var memberBankAccounts = _snapshot.SettlementAccounts.Where(x => x.MemberId == memberId);

            var matchedSettlementAccountInfo = memberBankAccounts.FirstOrDefault(x => x.MarketId == marketId);

            if (matchedSettlementAccountInfo != null)
            {
                return matchedSettlementAccountInfo.BankAccount ?? string.Empty;
            }

            return memberBankAccounts.FirstOrDefault(x => x.MarketId == Const.MemberSettlementAccount_Default.Market_All)?.BankAccount ?? string.Empty;
        }

        public DateTime GetNextWorkingDate(string? calendarId, DateTime date, int step) => CalculateNextWorkingDate(calendarId, date, step);
        public DateTime GetNextWorkingDate(string marketId, DateTime currentDate) => CalculateNextWorkingDate(marketId, currentDate);
        public DateTime GetCurrentWorkingDate() => DateTime.Now.Date;
        public DateTime GetPreviousWorkingDate(string marketId, DateTime currentDate) => CalculatePreviousWorkingDate(marketId, currentDate);

        private async Task<MemorySnapshot> BuildSnapshotAsync()
        {
            var accountTask = LoadAccountsAsync();
            var instrumentTask = LoadInstrumentsAsync();
            var marketTask = LoadMarketsAsync();
            var memberTask = LoadMembersAsync();
            var settlementTask = LoadMemberSettlementAccountAsync();
            var assetTask = LoadAssetDefinitionsAsync();
            var calendarTask = LoadCalendarsAsync();

            await Task.WhenAll([
                accountTask,
                instrumentTask,
                marketTask,
                memberTask,
                settlementTask,
                assetTask,
                calendarTask,
            ]);

            var accounts = await accountTask;
            var instruments = await instrumentTask;
            var markets = await marketTask;
            var members = await memberTask;
            var settlement = await settlementTask;
            var assets = await assetTask;
            var calendars = await calendarTask;


            return new MemorySnapshot
            {
                Accounts = accounts,
                Instruments = instruments,
                Markets = markets,
                Members = members,
                SettlementAccounts = settlement,
                Assets = assets,
                Calendars = calendars,

                AccountMapById = accounts.ToDictionary(x => x.AccountId),
                InstrumentMapById = instruments.ToDictionary(x => x.InstrumentId),
                MarketMapById = markets.ToDictionary(x => x.MarketId),
                MemberMapById = members.ToDictionary(x => x.MemberId),
                AssetMapById = assets.ToDictionary(x => x.AssetId),
                CalendarMapById = calendars.ToDictionary(x => x.CalendarId)
            };
        }

        public async Task Reload()
        {
            var newSnapshot = await BuildSnapshotAsync();
            Interlocked.Exchange(ref _snapshot, newSnapshot);
        }

        public async Task Initialize()
        {
            var newSnapshot = await BuildSnapshotAsync();
            Interlocked.Exchange(ref _snapshot, newSnapshot);
        }

        private async Task<List<Account>> LoadAccountsAsync()
        {
            return await _dataAccess.GetAccountsAsync();
        }

        private async Task<List<Instrument>> LoadInstrumentsAsync()
        {
            return await _dataAccess.GetInstrumentsAsync();
        }

        private async Task<List<Market>> LoadMarketsAsync()
        {
            return await _dataAccess.GetMarketsAsync();
        }

        private async Task<List<Member>> LoadMembersAsync()
        {
            return await _dataAccess.GetMembersAsync();
        }

        private async Task<List<MemberSettlementAccount>> LoadMemberSettlementAccountAsync()
        {
            return await _dataAccess.GetMemberSettlementAccountAsync();
        }

        private async Task<List<AssetDefinition>> LoadAssetDefinitionsAsync()
        {
            return await _dataAccess.GetAssetsAsync();
        }

        private async Task<List<Calendar>> LoadCalendarsAsync()
        {
            return await _dataAccess.GetCalendarsAsync();
        }

        private DateTime CalculatePreviousWorkingDate(string marketId, DateTime currentDate)
        {
            var market = GetMarketById(marketId);
            if (string.IsNullOrWhiteSpace(market.MarketId))
            {
                return currentDate;
            }

            return CalculatePreviousWorkingDate(market.CalendarId, currentDate, 1);
        }

        private DateTime CalculatePreviousWorkingDate(string? calendarId, DateTime date, int step)
        {
            DateTime previousWorkingDate = date.AddDays(-step).Date;

            if (string.IsNullOrWhiteSpace(calendarId))
            {
                return previousWorkingDate;
            }

            if (!_snapshot.CalendarMapById.TryGetValue(calendarId, out var calendar) || string.IsNullOrEmpty(calendar.CalendarId) || calendar.CalendarDates.Count == 0)
            {
                return previousWorkingDate;
            }

            var calendarDateSet = calendar.CalendarDateSet;

            while (true)
            {
                if (calendarDateSet.Contains(previousWorkingDate)
                    || (previousWorkingDate.Date.DayOfWeek == DayOfWeek.Saturday && calendar.SaturdayType != Const.YN.Yes)
                    || (previousWorkingDate.Date.DayOfWeek == DayOfWeek.Sunday && calendar.SundayType != Const.YN.Yes)
                )
                {
                    previousWorkingDate.AddDays(-1);
                }
                else
                {
                    break;
                }
            }

            return previousWorkingDate;
        }

        private DateTime CalculateNextWorkingDate(string marketId, DateTime currentDate)
        {
            var market = GetMarketById(marketId);
            if (string.IsNullOrWhiteSpace(market.MarketId))
            {
                return currentDate;
            }

            return CalculateNextWorkingDate(market.CalendarId, currentDate, market.DayOffset);
        }

        private DateTime CalculateNextWorkingDate(string? calendarId, DateTime date, int step)
        {
            DateTime nextWorkingDate = date.AddDays(step).Date;

            if (string.IsNullOrWhiteSpace(calendarId))
            {
                return nextWorkingDate;
            }

            if (!_snapshot.CalendarMapById.TryGetValue(calendarId, out var calendar) || string.IsNullOrEmpty(calendar.CalendarId) || calendar.CalendarDates.Count == 0)
            {
                return nextWorkingDate;
            }

            var calendarDateSet = calendar.CalendarDateSet;

            while (true)
            {
                if (calendarDateSet.Contains(nextWorkingDate)
                    || (nextWorkingDate.Date.DayOfWeek == DayOfWeek.Saturday && calendar.SaturdayType != Const.YN.Yes)
                    || (nextWorkingDate.Date.DayOfWeek == DayOfWeek.Sunday && calendar.SundayType != Const.YN.Yes)
                )
                {
                    nextWorkingDate.AddDays(1);
                }
                else
                {
                    break;
                }
            }

            return nextWorkingDate;
        }

    }
}
