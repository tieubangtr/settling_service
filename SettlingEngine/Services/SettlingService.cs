using SettlingEngine.Constants;
using SettlingEngine.DataAccesses;
using SettlingEngine.MemoryDatas;
using SettlingEngine.Models;
using SettlingEngine.Models.Accountings;
using SettlingEngine.Models.Markets;
using SettlingEngine.Models.Settlements;
using SettlingEngine.Utils;
using System.Data;
using System.Text.Json;

namespace SettlingEngine.Services
{
    public interface ISettlingService
    {
        Task<BusinessReponse> Settle(string? marketId, string? debitCredit);
        Task<BusinessReponse> CreateMemberSettlement();
        Task<List<Market>> GetMarkets();
        Task<List<MemberSettlement>> GetMemberSettlements(string? marketId);

        Task Test();
    }

    internal class SettlingService : ISettlingService
    {
        private readonly IDataAccess _dataAccess;
        private readonly IMemoryInstance _memoryInstance;

        public SettlingService(IDataAccess dataAccess, IMemoryInstance memoryInstance)
        {
            _dataAccess = dataAccess;
            _memoryInstance = memoryInstance;
        }
        public async Task Test()
        {
            using var transaction = _dataAccess.GetDatabaseTransaction();

            var settleDate = DateTime.Now.Date;
            var accountAccounting = await _dataAccess.GetAccountAccountingsAsync(transaction.Transaction, Const.Settlement_Status.Settled, "D", settleDate);
            var updateOrderMatchesResult = await UpdateOrderMatchesAsync(transaction.Transaction, accountAccounting, settleDate);

        }

        public async Task<BusinessReponse> Settle(string? marketId, string? debitCredit)
        {
            if (string.IsNullOrEmpty(marketId) || string.IsNullOrEmpty(debitCredit))
            {
                return new() { Code = -1, Message = "Invalid memberId" };
            }

            try
            {
                // Get all current date member accounting
                var settleDate = DateTime.Now.Date;
                var memberAccountings = await _dataAccess.GetMemberAccountingsAsync(marketId, debitCredit, settleDate, Const.Settlement_Status.Unsettled);

                if (memberAccountings == null || memberAccountings.Count == 0)
                {
                    return new() { Code = -1, Message = "No member accounting to settle" };
                }

                // Settle ??
                using var transaction = _dataAccess.GetDatabaseTransaction();

                var updateMemberSettlementsResult = await UpdateMemberSettlementsAsync(transaction.Transaction, memberAccountings, settleDate);
                var updateMemberAccountingsResult = await UpdateMemberAccountingsAsync(transaction.Transaction, memberAccountings, settleDate);
                var updateAccountAccountingsResult = await UpdateAccountAccountingsAsync(transaction.Transaction, memberAccountings, settleDate);


                var accountAccounting = await _dataAccess.GetAccountAccountingsAsync(transaction.Transaction, Const.Settlement_Status.Settled, debitCredit, settleDate);
                var updateAccountBalancesResult = await UpdateAccountBalancesAsync(transaction.Transaction, accountAccounting, settleDate);
                var updateOrderMatchesResult = await UpdateOrderMatchesAsync(transaction.Transaction, accountAccounting, settleDate);

                Logger.log.Info($"Settle result: updateMemberSettlementsResult=[{updateMemberSettlementsResult}] " +
                    $"updateMemberAccountingsResult=[{updateMemberAccountingsResult}] updateAccountAccountingsResult=[{updateAccountAccountingsResult}]" +
                    $"updateAccountBalancesResult=[{updateAccountBalancesResult}] updateOrderMatchesResult=[{updateOrderMatchesResult}]");

                List<bool> results = [
                    updateMemberSettlementsResult,
                    updateMemberAccountingsResult,
                    updateAccountAccountingsResult,
                    updateAccountBalancesResult,
                    updateOrderMatchesResult
                ];

                if (results.Any(x => x == false))
                {
                    transaction.Rollback();
                    return new() { Code = -1, Message = "Settle failed" };
                }

                transaction.Commit();
                return new() { Code = 1, Message = "Success" };
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
            }

            return new() { Code = -1, Message = "Error occurred" };
        }

        public async Task<BusinessReponse> CreateMemberSettlement()
        {
            try
            {
                // Get all current date member accounting
                var createDate = DateTime.Now.Date;
                var status = Const.Settlement_Status.Unsettled;
                var memberAccountings = await _dataAccess.GetMemberAccountingIndayAsync(createDate.ToString("yyyyMMdd"), status);

                if (memberAccountings == null || memberAccountings.Count == 0)
                {
                    Logger.log.Warn($"CreateMemberSettlement - No member accountings found: createDate=[{createDate}] status=[{status}]");
                    return new() { Code = 1, Message = "No member accounting to create settlement" };
                }

                // Settle ??
                var memberSettlements = GetMemberSettlements(memberAccountings);

                using var transaction = _dataAccess.GetDatabaseTransaction();

                var insertMemberSettlementsResult = await _dataAccess.InsertMemberSettlementsAsync(transaction.Transaction, memberSettlements);

                List<bool> results = [
                    insertMemberSettlementsResult,
                ];

                if (results.Any(x => x == false))
                {
                    transaction.Rollback();
                    Logger.log.Error($"CreateMemberSettlement - Insert member settlements failed: settlementCount=[{memberSettlements.Count}]");
                    return new() { Code = -1, Message = "Insert member settlements failed" };
                }

                transaction.Commit();
                return new() { Code = 1, Message = "Success" };
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex);
                return new() { Code = -2, Message = ex.Message };
            }
        }

        private List<MemberSettlement> GetMemberSettlements(List<MemberAccounting> memberAccountings)
        {
            if (memberAccountings == null || memberAccountings.Count == 0)
            {
                return [];
            }

            try
            {
                var memberSettlements = new List<MemberSettlement>();
                foreach (var item in memberAccountings)
                {
                    var settlementId = Util.GenerateId(Const.MemberSettlement_IdentityPrefix.Settlement);
                    var settlementAccountCCP = _memoryInstance.GetMemberSettlementAccount(Const.MemberSettlementAccount_Default.CCPMember, item.MarketId);
                    var settlementAccountMember = _memoryInstance.GetMemberSettlementAccount(item.MemberId, item.MarketId);

                    var descriptionHeader = item.Debit_Credit == Const.Debit_Credit.Debit ? "Ghi nợ" : "Ghi có";
                    var description = descriptionHeader + $" {item.AssetId} ngày {DateTime.Now:dd/MM/yyyy}";

                    memberSettlements.Add(new()
                    {
                        SettlementId = settlementId,
                        SettleDate = item.SettleDate.Date,
                        MemberId = item.MemberId,
                        MarketId = item.MarketId,
                        AssetId = item.AssetId,
                        Debit_Credit = item.Debit_Credit,
                        SettlementAccountCCP = settlementAccountCCP,
                        SettlementAccountMember = settlementAccountMember,
                        MemberAccountingId = item.MemberAccountingId,
                        Description = description,
                        Amount = item.AccountingValue,
                        Status = Const.Settlement_Status.Unsettled,
                        CreateBy = "Sys",
                        CreateDate = DateTime.Now,
                    });
                }

                return memberSettlements;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }

        private async Task<bool> UpdateMemberSettlementsAsync(IDbTransaction transaction, List<MemberAccounting> memberAccountings, DateTime settleDate)
        {
            if (memberAccountings == null || memberAccountings.Count == 0)
            {
                Logger.log.Warn($"Settle - Update membersettlement - No member accounting from input: memberAccountingCount=[{memberAccountings?.Count}]");
                return true;
            }
            try
            {
                var memberAccountingIds = memberAccountings.Select(x => x.MemberAccountingId).Distinct().ToList();
                var memberSettlements = await _dataAccess.GetMemberSettlementsAsync(transaction, memberAccountingIds, Const.Settlement_Status.Unsettled);
                if (memberSettlements == null || memberSettlements.Count == 0)
                {
                    Logger.log.Warn($"Settle - Update member settlement - No data found: memberSettlementCount=[{memberSettlements?.Count}]");
                    return true;
                }

                memberSettlements.ForEach(x =>
                {
                    x.Status = Const.Settlement_Status.Settled;
                    x.LastChangeBy = "Sys";
                    x.LastChangeDate = DateTime.Now;
                });

                var updateResult = await _dataAccess.UpdateMemberSettlementsAsync(transaction, memberSettlements);

                Logger.log.Info($"Settle - Update member settlement - Done: updateResult=[{updateResult}] memberSettlementCount=[{memberSettlements?.Count}]");

                return updateResult;

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        private async Task<bool> UpdateMemberAccountingsAsync(IDbTransaction transaction, List<MemberAccounting> memberAccountings, DateTime settleDate)
        {
            if (memberAccountings == null || memberAccountings.Count == 0)
            {
                Logger.log.Warn($"Settle - Update member accounting - No member accounting from input: memberAccountingCount=[{memberAccountings?.Count}]");

                return true;
            }
            try
            {
                memberAccountings.ForEach(x =>
                {
                    x.Status = Const.Settlement_Status.Settled;
                    x.LastChangeBy = "Sys";
                    x.LastChangeDate = DateTime.Now;
                });

                var updateResult = await _dataAccess.UpdateMemberAccountingsAsync(transaction, memberAccountings);

                Logger.log.Info($"Settle - Update member accounting - Done: updateResult=[{updateResult}] memberSettlementCount=[{memberAccountings?.Count}]");

                return updateResult;

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        private async Task<bool> UpdateAccountAccountingsAsync(IDbTransaction transaction, List<MemberAccounting> memberAccountings, DateTime settleDate)
        {
            if (memberAccountings == null || memberAccountings.Count == 0)
            {
                Logger.log.Warn($"Settle - Update account accounting - No member accounting from input: memberAccountingCount=[{memberAccountings?.Count}]");

                return true;
            }
            try
            {
                var memberIds = memberAccountings.Select(ma => ma.MemberId).Distinct().ToList();
                var accountAccountings = await _dataAccess.GetAccountAccountingsAsync(transaction, memberIds, settleDate);

                if (accountAccountings == null || accountAccountings.Count == 0)
                {
                    Logger.log.Warn($"Settle - Update account accounting - No account accounting found: memberIds=[{JsonSerializer.Serialize(memberIds)}] settleDate=[{settleDate}] accountAccountingCount=[{accountAccountings?.Count}]");
                    return true;
                }

                accountAccountings.ForEach(x =>
                {
                    x.Status = Const.Settlement_Status.Settled;
                    x.LastChangeBy = "Sys";
                    x.LastChangeDate = DateTime.Now;
                });

                var updateResult = await _dataAccess.UpdateAccountAccountingsAsync(transaction, accountAccountings);

                Logger.log.Info($"Settle - Update account accounting - Done: updateResult=[{updateResult}] accountAccountingCount=[{accountAccountings?.Count}]");

                return updateResult;

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        private async Task<bool> UpdateAccountBalancesAsync(IDbTransaction transaction, List<AccountAccounting> accountAccountings, DateTime settleDate)
        {
            if (accountAccountings == null || accountAccountings.Count == 0)
            {
                Logger.log.Warn($"Settle - Update account balance - No account accounting from input: accountAccountingCount=[{accountAccountings?.Count}]");

                return true;
            }

            try
            {
                var accountBalances = await _dataAccess.GetAccountBalancesAsync(transaction);
                if (accountBalances == null || accountBalances.Count == 0)
                {
                    Logger.log.Warn($"Settle - Update account balance - No account balance found: accountBalanceCount=[{accountBalances?.Count}]");

                    return true;
                }

                var accountBalanceLookUp = accountBalances
                    .Where(x => accountAccountings.Any(y => y.AccountId == x.AccountId && y.AssetId == x.AssetId))
                    .ToDictionary(x => (x.AccountId, x.AssetId), x => x);

                foreach (var item in accountAccountings)
                {
                    var marketDayOffset = _memoryInstance.GetMarketById(item.MarketId).DayOffset;
                    if (!accountBalanceLookUp.TryGetValue((item.AccountId, item.AssetId), out var accountBalance))
                    {
                        continue;
                    }

                    switch (item.Debit_Credit)
                    {
                        case Const.Debit_Credit.Credit:
                            //Tăng balance
                            accountBalance.Balance += item.AccountingValue;

                            switch (marketDayOffset)
                            {
                                case Const.Market_DayOffset.T0: accountBalance.MatchBuy0 -= item.AccountingValue; break;
                                case Const.Market_DayOffset.T1: accountBalance.MatchBuy1 -= item.AccountingValue; break;
                                case Const.Market_DayOffset.T2: accountBalance.MatchBuy2 -= item.AccountingValue; break;
                            }
                            break;

                        case Const.Debit_Credit.Debit:
                            //Giảm balance và holdForTrade
                            accountBalance.Balance -= item.AccountingValue;
                            accountBalance.HoldForTrade -= item.AccountingValue;

                            switch (marketDayOffset)
                            {
                                case Const.Market_DayOffset.T0: accountBalance.MatchSell0 -= item.AccountingValue; break;
                                case Const.Market_DayOffset.T1: accountBalance.MatchSell1 -= item.AccountingValue; break;
                                case Const.Market_DayOffset.T2: accountBalance.MatchSell2 -= item.AccountingValue; break;
                            }

                            break;
                    }
                }

                var processedAccountBalances = accountBalanceLookUp.Values.ToList();

                var updateResult = await _dataAccess.UpdateAccountBalancesAsync(transaction, processedAccountBalances);

                Logger.log.Info($"Settle - Update account balance - Done: updateResult=[{updateResult}] processedAccountBalances=[{processedAccountBalances?.Count}]");

                return updateResult;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        private async Task<bool> UpdateOrderMatchesAsync(IDbTransaction transaction, List<AccountAccounting> accountAccountings, DateTime settleDate)
        {
            if (accountAccountings == null || accountAccountings.Count == 0)
            {
                Logger.log.Warn($"Settle - Update order match - No account accounting from input: settleDate=[{settleDate}] accountAccountingCount=[{accountAccountings?.Count}]");

                return true;
            }

            try
            {
                List<string> marketSettleKeys = [.. accountAccountings.Select(x => $"{x.MarketId}-{x.SettleDate.ToString("yyyyMMdd")}").Distinct()];

                var orderMatches = await _dataAccess.GetOrderMatchesAsync(transaction, marketSettleKeys);
                if (orderMatches == null || orderMatches.Count == 0)
                {
                    Logger.log.Warn($"Settle - Update order match - No order match found: marketSettleKeys=[{JsonSerializer.Serialize(marketSettleKeys)}] orderMatcheCount=[{orderMatches?.Count}]");

                    return true;
                }

                orderMatches.ForEach(x =>
                {
                    x.Is_Payment_Processed = Const.YN.Yes;
                });

                var updateResult = await _dataAccess.UpdateOrderMatchesAsync(transaction, orderMatches);

                Logger.log.Info($"Settle - Update order match - Done: updateResult=[{updateResult}] orderMatches=[{orderMatches?.Count}]");

                return updateResult;
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return false;
        }

        public Task<List<Market>> GetMarkets()
        {
            try
            {
                var markets = _memoryInstance.GetMarkets();

                return Task.FromResult(markets.ToList());
            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return Task.FromResult(new List<Market>());
        }

        public async Task<List<MemberSettlement>> GetMemberSettlements(string? marketId)
        {
            if (string.IsNullOrEmpty(marketId))
            {
                return [];
            }

            try
            {
                var marketInfo = _memoryInstance.GetMarketById(marketId);

                if (marketInfo == null)
                {
                    return [];
                }

                var currentDate = _memoryInstance.GetCurrentWorkingDate();
                var previousWorkingDate = _memoryInstance.GetPreviousWorkingDate(marketId, currentDate);
                List<DateTime> dates = [currentDate, previousWorkingDate];

                return await _dataAccess.GetMemberSettlementsAsync(marketId, dates);

            }
            catch (Exception ex)
            {
                Logger.log.Error(ex.Message);
            }

            return [];
        }
    }
}
