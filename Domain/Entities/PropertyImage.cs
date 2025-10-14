using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace Domain.Entities
{
  public class PropertyImage
  {
    [BsonId]
    [BsonRepresentation(BsonType.ObjectId)]
    public string IdPropertyImage { get; set; }

    [BsonRepresentation(BsonType.ObjectId)]
    [BsonElement("idProperty")]
    public string IdProperty { get; set; }

    [BsonElement("file")]
    public string File { get; set; } // URL o base64

    [BsonElement("enabled")]
    public bool Enabled { get; set; }
  }
}
