using System.Text.Json.Serialization;
using CORE.ENSEK.Attributes;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace CORE.ENSEK.Models;

[BsonCollection("meterreadings")]
public class MeterReading : Document
{
    [BsonRepresentation(BsonType.Int32)]
    public int? AccountId { get; set; }

    [JsonPropertyName("meterValue")] public string MeterValue { get; set; }
    [JsonPropertyName("meterDate")] public DateTime MeterReadAt { get; set; }
}