using MongoDB.Driver;
using Domain.Entities;
using Bogus;

namespace RealState.Infrastructure.Persistence
{
    public static class DatabaseSeeder
    {
        public static async Task SeedAsync(IMongoDatabase database)
        {
            var owners = database.GetCollection<Owner>("Owner");
            var properties = database.GetCollection<Property>("Property");
            var images = database.GetCollection<PropertyImage>("PropertyImage");
            var traces = database.GetCollection<PropertyTrace>("PropertyTrace");

            if (await owners.CountDocumentsAsync(_ => true) > 0)
                return;

            var random = new Random();

            var ownerFaker = new Faker<Owner>("en_US")
                .RuleFor(o => o.Name, f => f.Name.FullName())
                .RuleFor(o => o.Address, f => f.Address.FullAddress())
                .RuleFor(o => o.Photo, f => $"https://randomuser.me/api/portraits/{(f.Random.Bool() ? "men" : "women")}/{f.Random.Number(1, 90)}.jpg")
                .RuleFor(o => o.Birthday, f => f.Date.Past(40, DateTime.Now.AddYears(-25)));

            var ownersList = ownerFaker.Generate(15);
            await owners.InsertManyAsync(ownersList);

            string[] propertyTypes = { "Modern Apartment", "Luxury Villa", "Cozy Cabin", "Beach House", "Penthouse", "Suburban Home", "Studio Loft", "Mountain Retreat" };
            string[] cities = { "Miami, FL", "Los Angeles, CA", "New York, NY", "Austin, TX", "Seattle, WA", "Denver, CO", "Chicago, IL", "Phoenix, AZ", "Atlanta, GA" };

            var propertyFaker = new Faker<Property>("en_US")
                .RuleFor(p => p.Name, f => $"{f.PickRandom(propertyTypes)} in {f.PickRandom(cities)}")
                .RuleFor(p => p.Address, f => f.Address.StreetAddress())
                .RuleFor(p => p.Price, f => f.Random.Number(120_000, 1_500_000))
                .RuleFor(p => p.CodeInternal, f => $"PROP-{f.Random.Number(1000, 9999)}")
                .RuleFor(p => p.Year, f => f.Date.Past(10).Year)
                .RuleFor(p => p.IdOwner, f => f.PickRandom(ownersList).IdOwner);

            var propertiesList = propertyFaker.Generate(random.Next(30, 60));
            await properties.InsertManyAsync(propertiesList);

            // 🖼️ Faker para imágenes (URLs reales de Unsplash)
            string[] imageUrls = {
                "https://images.unsplash.com/photo-1512917774080-9991f1c4c750?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1740",
                "https://images.unsplash.com/photo-1723110994499-df46435aa4b3?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1758",
                "https://images.unsplash.com/photo-1610569244414-5e7453a447a8?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1548",
                "https://images.unsplash.com/photo-1719299225324-301bad5c333c?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1740",
                "https://images.unsplash.com/photo-1512915922686-57c11dde9b6b?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1746",
                "https://plus.unsplash.com/premium_photo-1746387628298-af5695a3f935?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1932",
                "https://images.unsplash.com/photo-1599809275695-5e96ca83bc43?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=1740",
                "https://plus.unsplash.com/premium_photo-1689609950069-2961f80b1e70?ixlib=rb-4.1.0&ixid=M3wxMjA3fDB8MHxwaG90by1wYWdlfHx8fGVufDB8fHx8fA%3D%3D&auto=format&fit=crop&q=80&w=774"
            };

            var imageFaker = new Faker<PropertyImage>("en_US")
                .RuleFor(i => i.IdProperty, f => f.PickRandom(propertiesList).IdProperty)
                .RuleFor(i => i.File, f => f.PickRandom(imageUrls))
                .RuleFor(i => i.Enabled, f => true);

            var imagesList = imageFaker.Generate(propertiesList.Count);
            await images.InsertManyAsync(imagesList);

            // 💰 Faker para trazas de venta
            var traceFaker = new Faker<PropertyTrace>("en_US")
                .RuleFor(t => t.IdProperty, f => f.PickRandom(propertiesList).IdProperty)
                .RuleFor(t => t.DateSale, f => f.Date.Past(1))
                .RuleFor(t => t.Name, f => "Previous Sale")
                .RuleFor(t => t.Value, f => f.Random.Number(100_000, 1_000_000))
                .RuleFor(t => t.Tax, f => f.Random.Number(2_000, 20_000));

            var tracesList = traceFaker.Generate(propertiesList.Count / 2);
            await traces.InsertManyAsync(tracesList);

            Console.WriteLine($"✅ Se insertaron {ownersList.Count} dueños, {propertiesList.Count} propiedades, {imagesList.Count} imágenes y {tracesList.Count} trazas.");
        }
    }
}
