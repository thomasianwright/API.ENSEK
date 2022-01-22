using MongoDB.Bson;

namespace CORE.ENSEK.Models;

public abstract class Document : IDocument
{
    public ObjectId Id { get; set; }

    public DateTime CreatedAt => Id.CreationTime;
}