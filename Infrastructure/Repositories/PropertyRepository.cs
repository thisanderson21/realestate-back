using Application.Interfaces;
using MongoDB.Driver;
using MongoDB.Bson;
using Application.DTOS;
using Application.Queries.GetAllProperties;

namespace Infrastructure.Persistence.Repositories
{
    public class PropertyRepository : IPropertyRepository
    {
        private readonly IMongoDbContext  _context;

        public PropertyRepository(IMongoDbContext context)
        {
            _context = context;
        }

        public async Task<List<PropertyListDto>> GetAllAsync(PropertyFilters filters)
        {
            var pipelineStages = new List<BsonDocument>();
            
            filters ??= new PropertyFilters();

            var matchFilter = BuildMatchFilter(filters);
            if (matchFilter != null)
            {
                pipelineStages.Add(matchFilter);
            }

            // Lookup de imágenes
            pipelineStages.Add(new BsonDocument("$lookup", new BsonDocument
            {
                { "from", "PropertyImage" },
                { "localField", "_id" },
                { "foreignField", "idProperty" },
                { "as", "images" }
            }));

            pipelineStages.Add(new BsonDocument("$addFields", new BsonDocument
            {
                { "firstImage", new BsonDocument("$first", 
                    new BsonDocument("$filter", new BsonDocument
                    {
                        { "input", "$images" },
                        { "cond", "$$this.enabled" }
                    })
                )}
            }));

            pipelineStages.Add(new BsonDocument("$project", new BsonDocument
            {
                { "_id", 1 },
                { "idOwner", 1 },
                { "name", 1 },
                { "address", 1 },
                { "price", 1 },
                { "image", "$firstImage.file" }
            }));

            return await _context.Properties
                .Aggregate<PropertyListDto>(pipelineStages)
                .ToListAsync();
            }

        private BsonDocument? BuildMatchFilter(PropertyFilters filters)
        {
            var conditions = new List<BsonDocument>();

            if (!string.IsNullOrWhiteSpace(filters.Search))
            {
                var regex = new BsonDocument("$regex", filters.Search)
                    .Add("$options", "i");

                var orConditions = new BsonArray
                {
                    new BsonDocument("name", regex),
                    new BsonDocument("address", regex)
                };

                conditions.Add(new BsonDocument("$or", orConditions));
            }

            if (filters.MinPrice.HasValue)
            {
                conditions.Add(new BsonDocument("price", 
                    new BsonDocument("$gte", filters.MinPrice.Value)));
            }

            if (filters.MaxPrice.HasValue)
            {
                conditions.Add(new BsonDocument("price", 
                    new BsonDocument("$lte", filters.MaxPrice.Value)));
            }

            if (!conditions.Any())
                return null;

            return new BsonDocument("$match", 
                new BsonDocument("$and", new BsonArray(conditions)));
        }
    }
}
