namespace Infrastructure.Persistence
{
  public class MongoDbSettings
  {
    public string ConnectionString { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public string PropertiesCollectionName { get; set; } = "Properties";

    public string PropertyImagesCollectionName { get; set; } = "PropertyImages";

    public string PropertyTracesCollectionName { get; set; } = "PropertyTraces";
    
    public string OwnersCollectionName { get; set; } = "Owners";
  }
}
