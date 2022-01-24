using System.Diagnostics.Metrics;
using CORE.ENSEK.Attributes;
using MongoDB.Bson.Serialization.Attributes;

namespace CORE.ENSEK.Models;

[BsonCollection("accounts")]
public class Account : Document
{
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public int? AccountId { get; set; }
    
    [BsonIgnore]
    public string DbId => Id.ToString();
}