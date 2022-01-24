using System.Globalization;
using System.Text.RegularExpressions;
using CORE.ENSEK.Models;
using CsvHelper;
using Microsoft.Extensions.Logging;

namespace CORE.ENSEK.Services;

public class CsvService
{
    private readonly ILogger<CsvService> _logger;

    public CsvService(ILogger<CsvService> logger)
    {
        _logger = logger;
    }

    public Task<(int failed, List<MeterReading> records)> ParseMeterReadings(string data)
    {
        var UserCsvData = Convert.FromBase64String(data);
        var records = new List<MeterReading>();
        var failed = 0;

        using var reader = new StreamReader(new MemoryStream(UserCsvData));
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                try
                {
                    var meterValue = Regex.Match(csv.GetField<string>("MeterReadValue"), "^([0-9]{5})$").Success;

                    if (meterValue)
                    {
                        var record = new MeterReading
                        {
                            AccountId = csv.GetField<int>("AccountId"),
                            MeterValue = csv.GetField<string>("MeterReadValue"),
                            MeterReadAt = DateTime.ParseExact(csv.GetField<string>("MeterReadingDateTime"), "dd/MM/yyyy HH:mm", CultureInfo.InvariantCulture),
                        };

                        records.Add(record);
                        continue;
                    }
                    
                    _logger.LogError($"Invalid Meter Read Value: {csv.GetField<string>("MeterReadValue")}");
                    failed++;
                }
                catch (Exception e)
                {
                    _logger.LogError(e.Message);
                    failed++;
                }
            }
        }

        return Task.FromResult((failed, records));
    }

    public Task<(int failed, List<Account> records)> ParseUserAccounts(string data)
    {
        var UserCsvData = Convert.FromBase64String(data);
        var records = new List<Account>();
        var failed = 0;

        using var reader = new StreamReader(new MemoryStream(UserCsvData));
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                try
                {
                    var record = new Account()
                    {
                        AccountId = csv.GetField<int>("AccountId"),
                        FirstName = csv.GetField("FirstName"),
                        LastName = csv.GetField("LastName"),
                    };

                    records.Add(record);
                }
                catch (Exception e)
                {
                    failed++;
                }
            }
        }

        return Task.FromResult((failed, records));
    }
}