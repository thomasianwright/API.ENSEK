using System.Diagnostics.Metrics;
using System.Globalization;
using System.Text.Json;
using CORE.ENSEK.Models;
using CORE.ENSEK.Repositories;
using Microsoft.Extensions.Logging;

namespace CORE.ENSEK.Services;

public class AccountService
{
    private readonly IMongoRepository<Account> _accountRepository;
    private readonly CsvService _csvService;
    private readonly ILogger<AccountService> _logger;
    private readonly IMongoRepository<MeterReading> _meterReadingRepository;

    public AccountService(IMongoRepository<Account> accountRepository, CsvService csvService,
        ILogger<AccountService> logger, IMongoRepository<MeterReading> meterReadingRepository)
    {
        _accountRepository = accountRepository;
        _csvService = csvService;
        _logger = logger;
        _meterReadingRepository = meterReadingRepository;
    }

    public async Task<(int failed, int success)> InsertReadings(string data)
    {
        var readings = await _csvService.ParseMeterReadings(data);
        var readingsGroup = readings.records.GroupBy(x => x.AccountId);
        var failed = readings.failed;

        foreach (var group in readingsGroup)
        {
            var account = await _accountRepository.FindOneAsync(x => x.AccountId == group.Key);

            if (account == null)
            {
                failed++;
                _logger.LogError("Account not found for account id: {0}", group.Key);
                continue;
            }


            var meterReading = group.OrderBy(x=> x.MeterReadAt).ToList();

            foreach (var reading in meterReading)
            {
                var exists = await _meterReadingRepository.FindOneAsync(x =>
                    x.MeterReadAt <= reading.MeterReadAt && x.AccountId == reading.AccountId && x.MeterValue == reading.MeterValue);
                
                var OlderReading = await _meterReadingRepository.FindOneAsync(x =>
                    x.MeterReadAt > reading.MeterReadAt && x.AccountId == reading.AccountId);
                
                if (exists != null || OlderReading != null)
                {
                    failed++;
                    _logger.LogError("Duplicate reading found for account id: {0}, {1}", group.Key, JsonSerializer.Serialize(reading));
                    continue;
                }

                await _meterReadingRepository.InsertOneAsync(reading);
            }

            await _accountRepository.ReplaceOneAsync(account);
        }

        return (failed, readings.records.Count - failed);
    }

    public async Task<int> InsertUserAccounts(string data)
    {
        var accounts = await _csvService.ParseUserAccounts(data);

        var failedCount = accounts.failed;

        foreach (var account in accounts.records)
        {
            var userExists = await _accountRepository.FindOneAsync(x => x.AccountId == account.AccountId);

            if (userExists != null)
            {
                failedCount++;
                continue;
            }

            await _accountRepository.InsertOneAsync(account);
        }

        return failedCount;
    }

    public Task<IEnumerable<Account>> GetAllAccounts()
    {
        return Task.FromResult(_accountRepository.FilterBy(_ => true));
    }

    public async Task<Account> GetAccount(string accountId)
    {
        return await _accountRepository.FindByIdAsync(accountId);
    }
}