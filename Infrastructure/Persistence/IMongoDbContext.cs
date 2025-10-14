using Domain.Entities;
using MongoDB.Driver;

namespace Infrastructure.Persistence
{
    public interface IMongoDbContext
    {
        IMongoCollection<Property> Properties { get; }
        IMongoCollection<PropertyImage> PropertyImages { get; }
        IMongoCollection<PropertyTrace> PropertyTraces { get; }
        IMongoCollection<Owner> Owners { get; }
    }
}