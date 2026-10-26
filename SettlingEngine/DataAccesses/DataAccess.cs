using DapperLib;
using SettlingEngine.Constants;
using SettlingEngine.Helpers;
using SettlingEngine.Models;
using SettlingEngine.Models.Accountings;
using SettlingEngine.Models.Accounts;
using SettlingEngine.Models.Calendars;
using SettlingEngine.Models.Instruments;
using SettlingEngine.Models.Markets;
using SettlingEngine.Models.Settlements;
using SettlingEngine.Utils;
using System.Data;

namespace SettlingEngine.DataAccesses
{
    internal interface IDataAccess
    {
        ITransactionHelper GetDatabaseTransaction();

        Task<List<Account>> GetAccountsAsync();
        Task<List<AccountBalance>> GetAccountBalancesAsync(IDbTransaction transaction);
        Task<List<Instrument>> GetInstrumentsAsync();
        Task<List<Member>> GetMembersAsync();
        Task<List<Market>> GetMarketsAsync();
        Task<List<AssetDefinition>> GetAssetsAsync();
        Task<List<Calendar>> GetCalendarsAsync();
        Task<List<OrderMatch>> GetOrderMatchesAsync(IDbTransaction transaction, List<string> marketSettleKeys);

        Task<List<MemberAccounting>> GetMemberAccountingsAsync(string marketId, string debitCredit, DateTime settleDate, string status);
        Task<List<MemberAccounting>> GetMemberAccountingIndayAsync(string createDate, string status);
        Task<List<MemberAccounting>> GetMemberAccountingsAsync(DateTime settleDate, string status);

        Task<List<MemberSettlement>> GetMemberSettlementsAsync(DateTime settleDate, string status);
        Task<List<MemberSettlement>> GetMemberSettlementsAsync(string marketId, List<DateTime> dates);
        Task<List<MemberSettlement>> GetMemberSettlementsAsync(IDbTransaction transaction, DateTime settleDate, string status);
        Task<List<MemberSettlement>> GetMemberSettlementsAsync(IDbTransaction transaction, List<string> memberAccountingIds, string status);
        Task<List<MemberSettlementAccount>> GetMemberSettlementAccountAsync();
        Task<List<AccountAccounting>> GetAccountAccountingsAsync(IDbTransaction transaction, List<string> memberIds, DateTime settleDate);
        Task<List<AccountAccounting>> GetAccountAccountingsAsync(IDbTransaction transaction, string status, string debitCredit, DateTime settleDate);
        Task<bool> InsertMemberSettlementsAsync(IDbTransaction transaction, List<MemberSettlement> memberSettlements);
        Task<bool> UpdateMemberSettlementsAsync(IDbTransaction transaction, List<MemberSettlement> memberSettlements);
        Task<bool> UpdateMemberAccountingsAsync(IDbTransaction transaction, List<MemberAccounting> memberAccountings);
        Task<bool> UpdateAccountAccountingsAsync(IDbTransaction transaction, List<AccountAccounting> accountAccountings);
        Task<bool> UpdateAccountBalancesAsync(IDbTransaction transaction, List<AccountBalance> accountBalances);
        Task<bool> UpdateOrderMatchesAsync(IDbTransaction transaction, List<OrderMatch> orderMatches);
    }

    internal class DataAccess : IDataAccess
    {
        private readonly IOracleHelper _oracleHelper;

        public DataAccess(IOracleHelper oracleHelper)
        {
            _oracleHelper = oracleHelper;
        }

        public ITransactionHelper GetDatabaseTransaction()
        {
            var dbContext = _oracleHelper.CreateNewDbContext();
            return new TransactionHelper(dbContext.Connection);
        }

        public async Task<List<Account>> GetAccountsAsync()
        {
            using var dbContext = _oracleHelper.CreateNewDbContext();

            var accounts = await dbContext.From<Account>()
                .Where(a => a.RecordStatus == Const.RecordStatus.Active)
                .SelectAsync();

            return [.. accounts];
        }

        public async Task<List<AccountBalance>> GetAccountBalancesAsync(IDbTransaction transaction)
        {
            if (transaction.Connection == null)
            {
                return [];
            }

            try
            {
                var accountBalances = await transaction.Connection.GetAsync<AccountBalance>();

                return [.. accountBalances];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<Instrument>> GetInstrumentsAsync()
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var instruments = await dbContext.From<Instrument>()
                    .Where(a => a.RecordStatus == Const.RecordStatus.Active)
                    .SelectAsync();

                return [.. instruments];

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];

        }

        public async Task<List<Member>> GetMembersAsync()
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var members = await dbContext.From<Member>()
                    .Where(a => a.RecordStatus == Const.RecordStatus.Active)
                    .SelectAsync();

                return [.. members];

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<Market>> GetMarketsAsync()
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var markets = await dbContext.From<Market>()
                    .Where(a => a.RecordStatus == Const.RecordStatus.Active)
                    .SelectAsync();

                return [.. markets];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<AssetDefinition>> GetAssetsAsync()
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var assetDefinitions = await dbContext.From<AssetDefinition>()
                    .Where(a => a.RecordStatus == Const.RecordStatus.Active)
                    .SelectAsync();

                return [.. assetDefinitions];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<Calendar>> GetCalendarsAsync()
        {
            using var dbContext = _oracleHelper.CreateNewDbContext();

            var calendars = await dbContext.From<Calendar>()
                .Where(x => x.RecordStatus == Const.RecordStatus.Active)
                .SelectAsync();

            var calendarDates = await dbContext.From<CalendarDate>()
                .Where(x => x.RecordStatus == Const.RecordStatus.Active)
                .SelectAsync();

            var calendarList = calendars.ToList();

            var calendarDateLookup = new Dictionary<string, List<CalendarDate>>();
            if (calendarDates.Any())
            {
                calendarDateLookup = calendarDates
                    .GroupBy(x => x.CalendarId)
                    .ToDictionary(x => x.Key, x => x.ToList());
            }

            foreach (var item in calendarList)
            {
                if (!calendarDateLookup.TryGetValue(item.CalendarId, out var calendarDateValue))
                {
                    continue;
                }

                item.CalendarDates = calendarDateValue;
                item.CalendarDateSet = [.. calendarDateValue.Where(x => x.DataType == Const.CalendarDate_DataType.Y).Select(x => Util.DateStringToDateTime(x.DateValue))];
            }

            return calendarList;
        }



        public async Task<List<OrderMatch>> GetOrderMatchesAsync(IDbTransaction transaction, List<string> marketSettleKeys)
        {
            if (transaction.Connection == null)
            {
                return [];
            }

            try
            {
                var orderMatches = await transaction.Connection.GetViewAsync<OrderMatch>(a => marketSettleKeys.Contains(a.MarketSettleKey));

                return [.. orderMatches];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberAccounting>> GetMemberAccountingsAsync(string marketId, string debitCredit, DateTime settleDate, string status)
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var memberAccountings = await dbContext.Connection.GetViewAsync<MemberAccounting>(a => a.MarketId == marketId
                        && a.SettleDate == settleDate
                        && a.Debit_Credit == debitCredit
                        && a.Status == status);

                return [.. memberAccountings];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberAccounting>> GetMemberAccountingIndayAsync(string createDate, string status)
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var memberAccountings = await dbContext.Connection
                    .GetViewAsync<MemberAccounting>(x => x.CreateDateStr == createDate && x.Status == status);

                return [.. memberAccountings];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberAccounting>> GetMemberAccountingsAsync(DateTime settleDate, string status)
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var memberAccountings = await dbContext.Connection
                    .GetViewAsync<MemberAccounting>(a => a.SettleDate == settleDate && a.Status == status);

                return [.. memberAccountings];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberSettlement>> GetMemberSettlementsAsync(DateTime settleDate, string status)
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var memberSettlements = await dbContext.From<MemberSettlement>()
                    .Where(a => a.CreateDate == settleDate && a.Status == status)
                    .SelectAsync();

                return [.. memberSettlements];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberSettlement>> GetMemberSettlementsAsync(string marketId, List<DateTime> dates)
        {
            try
            {
                using var dbContext = _oracleHelper.CreateNewDbContext();

                var memberSettlements = await dbContext.Connection
                    .GetViewAsync<MemberSettlement>(new Dictionary<string, object>
                    {
                        { nameof(MemberSettlement.DateString), dates.Select(x => x.ToString("yyyyMMdd")).ToList() },
                        { nameof(MemberSettlement.MarketId), marketId }
                    });

                return [.. memberSettlements];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberSettlement>> GetMemberSettlementsAsync(IDbTransaction transaction, DateTime settleDate, string status)
        {
            if (transaction.Connection == null)
            {
                return [];
            }

            try
            {
                var memberSettlements = await transaction.Connection.GetAsync<MemberSettlement>(a =>
                    a.CreateDate == settleDate
                    && a.Status == status);

                return [.. memberSettlements];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberSettlement>> GetMemberSettlementsAsync(IDbTransaction transaction, List<string> memberAccountingIds, string status)
        {
            if (transaction.Connection == null)
            {
                return [];
            }

            try
            {
                var memberSettlements = await transaction.Connection.GetAsync<MemberSettlement>(a =>
                    memberAccountingIds.Contains(a.MemberAccountingId)
                    && a.Status == status);

                return [.. memberSettlements];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }


        public async Task<List<AccountAccounting>> GetAccountAccountingsAsync(IDbTransaction transaction, List<string> memberIds, DateTime settleDate)
        {
            if (transaction.Connection == null)
            {
                return [];
            }

            try
            {
                var accountAccountings = await transaction.Connection.GetAsync<AccountAccounting>(a => memberIds.Contains(a.MemberId)
                    && a.SettleDate == settleDate
                    && a.Status == Const.Settlement_Status.Unsettled);

                return [.. accountAccountings];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<AccountAccounting>> GetAccountAccountingsAsync(IDbTransaction transaction, string status, string debitCredit, DateTime settleDate)
        {
            if (transaction.Connection == null)
            {
                return [];
            }

            try
            {
                var accountAccountings = await transaction.Connection.GetAsync<AccountAccounting>(a =>
                    a.SettleDate == settleDate && a.Debit_Credit == debitCredit
                    && a.Status == status);

                return [.. accountAccountings];
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        public async Task<List<MemberSettlementAccount>> GetMemberSettlementAccountAsync()
        {
            using var dbContext = _oracleHelper.CreateNewDbContext();

            var memberSettlementAccounts = await dbContext.From<MemberSettlementAccount>()
                .Where(a => a.RecordStatus == Const.RecordStatus.Active)
                .SelectAsync();

            return [.. memberSettlementAccounts];
        }

        public async Task<bool> InsertMemberSettlementsAsync(IDbTransaction transaction, List<MemberSettlement> memberSettlements)
        {
            if (memberSettlements == null || memberSettlements.Count == 0)
            {
                return true;
            }

            try
            {
                if (transaction.Connection == null)
                {
                    return false;
                }

                var insertedCount = await transaction.Connection.InsertAsync(memberSettlements, transaction: transaction);

                return insertedCount == memberSettlements.Count;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }

            return false;
        }

        public async Task<bool> UpdateMemberSettlementsAsync(IDbTransaction transaction, List<MemberSettlement> memberSettlements)
        {
            var updatedCount = 0;
            if (memberSettlements.Count == 0)
            {
                return true;
            }

            try
            {
                if (transaction.Connection == null)
                {
                    return false;
                }

                foreach (var item in memberSettlements)
                {
                    updatedCount += await transaction.Connection.UpdateAsync<MemberSettlement>(x => new MemberSettlement
                    {
                        Status = item.Status,
                        LastChangeBy = item.LastChangeBy,
                        LastChangeDate = item.LastChangeDate,
                    }, x => x.SettlementId == item.SettlementId, transaction: transaction);
                }

                return updatedCount == memberSettlements.Count;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }


        public async Task<bool> UpdateAccountAccountingsAsync(IDbTransaction transaction, List<AccountAccounting> accountAccountings)
        {
            var updatedCount = 0;
            if (accountAccountings.Count == 0)
            {
                return true;
            }

            try
            {
                if (transaction.Connection == null)
                {
                    return false;
                }

                foreach (var item in accountAccountings)
                {
                    updatedCount += await transaction.Connection.UpdateAsync<AccountAccounting>(x => new AccountAccounting
                    {
                        Status = item.Status,
                        LastChangeBy = item.LastChangeBy,
                        LastChangeDate = item.LastChangeDate,
                    }, x => x.AccountAccountingId == item.AccountAccountingId, transaction: transaction);
                }

                return updatedCount == accountAccountings.Count;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        public async Task<bool> UpdateAccountBalancesAsync(IDbTransaction transaction, List<AccountBalance> accountBalances)
        {
            var updatedCount = 0;
            if (accountBalances.Count == 0)
            {
                return true;
            }

            try
            {
                if (transaction.Connection == null)
                {
                    return false;
                }

                foreach (var item in accountBalances)
                {
                    updatedCount += await transaction.Connection.UpdateAsync<AccountBalance>(x => new AccountBalance
                    {
                        Balance = item.Balance,
                        MatchBuy0 = item.MatchBuy0,
                        MatchBuy1 = item.MatchBuy1,
                        MatchBuy2 = item.MatchBuy2,
                        MatchSell0 = item.MatchSell0,
                        MatchSell1 = item.MatchSell1,
                        MatchSell2 = item.MatchSell2,
                        HoldForTrade = item.HoldForTrade,
                    }, x => x.AccountId == item.AccountId && x.AssetId == item.AssetId, transaction: transaction);
                }

                return updatedCount == accountBalances.Count;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        public async Task<bool> UpdateMemberAccountingsAsync(IDbTransaction transaction, List<MemberAccounting> memberAccountings)
        {
            var updatedCount = 0;
            if (memberAccountings.Count == 0)
            {
                return true;
            }

            try
            {
                if (transaction.Connection == null)
                {
                    return false;
                }

                foreach (var item in memberAccountings)
                {
                    updatedCount += await transaction.Connection.UpdateAsync<MemberAccounting>(x => new MemberAccounting
                    {
                        Status = item.Status,
                        LastChangeBy = item.LastChangeBy,
                        LastChangeDate = item.LastChangeDate,
                    }, x => x.MemberAccountingId == item.MemberAccountingId, transaction: transaction);
                }

                return updatedCount == memberAccountings.Count;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        public async Task<bool> UpdateOrderMatchesAsync(IDbTransaction transaction, List<OrderMatch> orderMatches)
        {
            var updatedCount = 0;
            if (orderMatches.Count == 0)
            {
                return true;
            }

            try
            {
                if (transaction.Connection == null)
                {
                    return false;
                }

                foreach (var item in orderMatches)
                {
                    updatedCount += await transaction.Connection.UpdateAsync<OrderMatch, object>(item, new { orderId = item.OrderId }, transaction: transaction);
                }

                return updatedCount == orderMatches.Count;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }
    }
}
