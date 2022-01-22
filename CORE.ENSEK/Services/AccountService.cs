using System.Text.Json;
using CORE.ENSEK.Models;
using CORE.ENSEK.Repositories;

namespace CORE.ENSEK.Services;

public class AccountService
{
    private readonly IMongoRepository<Account> _accountRepository;
    private readonly CsvService _csvService;

    public AccountService(IMongoRepository<Account> accountRepository, CsvService csvService)
    {
        _accountRepository = accountRepository;
        _csvService = csvService;
    }

    public async Task<(int failed, int success)> InsertReadings(string data)
    {
        var readings = await _csvService.ParseMeterReadings(data);
        var readingsGroup = readings.records.GroupBy(x => x.AccountId);
        var failed = readings.failed;

        foreach (var group in readingsGroup)
        {
            Account? account = await _accountRepository.FindOneAsync(x => x.AccountId == group.Key);

            if (account == null)
            {
                failed++;
                continue;
            }
            
            account.Readings ??= new List<MeterReading>();
            
            var accountReadings = account.Readings.ToList();
            var meterReading = group.ToList();
            
            foreach (var reading in meterReading)
            {

                if (accountReadings.Select(x=> x.MeterValue == reading.MeterValue && x.MeterReadAt == reading.MeterReadAt).Any())
                {
                    failed++;
                    continue;
                }

                accountReadings.Add(reading);
            }

            account.Readings = accountReadings;
            
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
    
    public  Task<IEnumerable<Account>> GetAllAccounts()
    {
        return Task.FromResult(_accountRepository.FilterBy(_ => true));
    }
    
    public async Task<Account> GetAccount(string accountId)
    {
        return await _accountRepository.FindByIdAsync(accountId);
    }
    
}