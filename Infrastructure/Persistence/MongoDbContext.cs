using Domain.Entities;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace Infrastructure.Persistence
{
  public class MongoDbContext : IMongoDbContext
  {
    private readonly IMongoDatabase _database;
    private readonly MongoDbSettings _settings;

    public MongoDbContext(IOptions<MongoDbSettings> options)
    {
      _settings = options.Value;
      var client = new MongoClient(_settings.ConnectionString);
      _database = client.GetDatabase(_settings.DatabaseName);
    }

    public IMongoCollection<Property> Properties =>
        _database.GetCollection<Property>(_settings.PropertiesCollectionName);

    public IMongoCollection<PropertyImage> PropertyImages =>
          _database.GetCollection<PropertyImage>(_settings.PropertyImagesCollectionName);

    public IMongoCollection<PropertyTrace> PropertyTraces =>
        _database.GetCollection<PropertyTrace>(_settings.PropertyTracesCollectionName);

    public IMongoCollection<Owner> Owners =>
        _database.GetCollection<Owner>(_settings.OwnersCollectionName);
  }
}
