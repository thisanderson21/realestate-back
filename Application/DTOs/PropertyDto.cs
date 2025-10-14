using MongoDB.Bson.Serialization.Attributes;

namespace Application.DTOS;

public class PropertyListDto
{
    [BsonElement("_id")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string _id { get; set; }

    [BsonElement("idOwner")]
    [BsonRepresentation(MongoDB.Bson.BsonType.ObjectId)]
    public string IdOwner { get; set; }

    [BsonElement("name")]
    public string Name { get; set; }

    [BsonElement("address")]
    public string AddressProperty { get; set; }

    [BsonElement("price")]
    public decimal PriceProperty { get; set; }

    [BsonElement("image")]
    [BsonIgnoreIfNull]
    public string Image { get; set; }
}
