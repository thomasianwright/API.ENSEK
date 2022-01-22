using System.Text.Json.Serialization;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CORE.ENSEK.Models;

public class MeterReading
{
    [BsonRepresentation(BsonType.Int32)]
    public int? AccountId { get; set; }

    [JsonPropertyName("meterValue")] public string MeterValue { get; set; }
    [JsonPropertyName("meterDate")] public DateTime MeterReadAt { get; set; }
}