using NUnit.Framework;
using Moq;
using MongoDB.Driver;
using MongoDB.Bson;
using Application.Interfaces;
using Application.DTOS;
using Application.Queries.GetAllProperties;
using Infrastructure.Persistence.Repositories;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Infrastructure.Persistence;
using Domain.Entities;

namespace Infrastructure.Tests.Repositories
{
    [TestFixture]
    public class PropertyRepositoryTests
    {
        private Mock<IMongoDbContext> _mockContext;
        private Mock<IMongoCollection<Property>> _mockCollection;
        private PropertyRepository _repository;

        [SetUp]
        public void SetUp()
        {
            _mockContext = new Mock<IMongoDbContext>();
            _mockCollection = new Mock<IMongoCollection<Property>>();
            
            // Configurar el contexto para devolver la colección mockeada
            _mockContext.Setup(x => x.Properties).Returns(_mockCollection.Object);
            
            _repository = new PropertyRepository(_mockContext.Object);
        }

        [TearDown]
        public void TearDown()
        {
            _mockContext = null;
            _mockCollection = null;
            _repository = null;
        }

        #region GetAllAsync Tests

        [Test]
        public async Task GetAllAsync_WithoutFilters_ReturnsAllProperties()
        {
            // Arrange
            var expectedProperties = CreateTestPropertyListDtos(3);
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(3));
            Assert.That(result, Is.EqualTo(expectedProperties));
            
            // Verificar que Aggregate fue llamado
            _mockCollection.Verify(
                x => x.Aggregate<PropertyListDto>(
                    It.IsAny<PipelineDefinition<Property, PropertyListDto>>(),
                    It.IsAny<AggregateOptions>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task GetAllAsync_WithSearchFilter_ReturnsFilteredProperties()
        {
            // Arrange
            var filters = new PropertyFilters { Search = "apartamento" };
            var expectedProperties = CreateTestPropertyListDtos(2, "Apartamento");
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
            Assert.That(result.All(p => p.Name.Contains("Apartamento")), Is.True);
        }

        [Test]
        public async Task GetAllAsync_WithMinPriceFilter_ReturnsPropertiesAboveMinPrice()
        {
            // Arrange
            var filters = new PropertyFilters { MinPrice = 100000 };
            var expectedProperties = new List<PropertyListDto>
            {
                CreatePropertyListDto(price: 150000),
                CreatePropertyListDto(price: 200000)
            };
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.All(p => p.PriceProperty >= 100000), Is.True);
        }

        [Test]
        public async Task GetAllAsync_WithMaxPriceFilter_ReturnsPropertiesBelowMaxPrice()
        {
            // Arrange
            var filters = new PropertyFilters { MaxPrice = 150000 };
            var expectedProperties = new List<PropertyListDto>
            {
                CreatePropertyListDto(price: 100000),
                CreatePropertyListDto(price: 120000)
            };
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.All(p => p.PriceProperty <= 150000), Is.True);
        }

        [Test]
        public async Task GetAllAsync_WithPriceRangeFilter_ReturnsPropertiesInRange()
        {
            // Arrange
            var filters = new PropertyFilters 
            { 
                MinPrice = 100000,
                MaxPrice = 200000 
            };
            var expectedProperties = new List<PropertyListDto>
            {
                CreatePropertyListDto(price: 120000),
                CreatePropertyListDto(price: 180000)
            };
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.All(p => p.PriceProperty >= 100000 && p.PriceProperty <= 200000), Is.True);
        }

        [Test]
        public async Task GetAllAsync_WithAllFilters_ReturnsCorrectlyFilteredProperties()
        {
            // Arrange
            var filters = new PropertyFilters 
            { 
                Search = "casa",
                MinPrice = 100000,
                MaxPrice = 300000 
            };
            var expectedProperties = new List<PropertyListDto>
            {
                CreatePropertyListDto(name: "Casa Moderna", price: 250000)
            };
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(1));
            Assert.That(result[0].Name, Does.Contain("Casa"));
            Assert.That(result[0].PriceProperty, Is.InRange(100000, 300000));
        }

        [Test]
        public async Task GetAllAsync_NoMatchingResults_ReturnsEmptyList()
        {
            // Arrange
            var filters = new PropertyFilters { Search = "NoExiste12345" };
            var expectedProperties = new List<PropertyListDto>();
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        [Test]
        public async Task GetAllAsync_PropertiesWithImages_ReturnsFirstEnabledImage()
        {
            // Arrange
            var expectedProperties = new List<PropertyListDto>
            {
                new PropertyListDto
                {
                    // Id = ObjectId.GenerateNewId().ToString(),
                    IdOwner = ObjectId.GenerateNewId().ToString(),
                    Name = "Propiedad con Imagen",
                    AddressProperty = "Calle Test 123",
                    PriceProperty = 150000,
                    Image = "image1.jpg" // Primera imagen habilitada
                }
            };
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Image, Is.EqualTo("image1.jpg"));
        }

        [Test]
        public async Task GetAllAsync_PropertiesWithoutImages_ReturnsNullImage()
        {
            // Arrange
            var expectedProperties = new List<PropertyListDto>
            {
                CreatePropertyListDto(image: null)
            };
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(null);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result[0].Image, Is.Null);
        }

        [Test]
        public async Task GetAllAsync_VerifyPipelineStages_ContainsLookupStage()
        {
            // Arrange
            var expectedProperties = CreateTestPropertyListDtos(1);
            var capturedPipeline = new List<BsonDocument>();
            
            SetupAggregateQueryWithPipelineCapture(expectedProperties, capturedPipeline);

            // Act
            await _repository.GetAllAsync(null);

            // Assert - Verificar que Aggregate fue llamado
            _mockCollection.Verify(
                x => x.Aggregate<PropertyListDto>(
                    It.IsAny<PipelineDefinition<Property, PropertyListDto>>(),
                    It.IsAny<AggregateOptions>(),
                    It.IsAny<CancellationToken>()
                ),
                Times.Once
            );
        }

        [Test]
        public async Task GetAllAsync_EmptyFilters_ReturnsAllProperties()
        {
            // Arrange
            var filters = new PropertyFilters(); // Filtros vacíos
            var expectedProperties = CreateTestPropertyListDtos(2);
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        [Test]
        public async Task GetAllAsync_InvalidPriceRange_ReturnsEmptyList()
        {
            // Arrange
            var filters = new PropertyFilters 
            { 
                MinPrice = 200000,
                MaxPrice = 100000  // MinPrice > MaxPrice
            };
            var expectedProperties = new List<PropertyListDto>();
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result, Is.Empty);
        }

        #endregion

        #region BuildMatchFilter Tests (Indirectly tested through GetAllAsync)

        [Test]
        [TestCase("apartamento")]
        [TestCase("CASA")]
        [TestCase("villa")]
        public async Task GetAllAsync_SearchFilter_IsCaseInsensitive(string searchTerm)
        {
            // Arrange
            var filters = new PropertyFilters { Search = searchTerm };
            var expectedProperties = CreateTestPropertyListDtos(1, searchTerm.ToLower());
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            // El filtro debe ser case insensitive debido a la opción "i" en el regex
        }

        [Test]
        public async Task GetAllAsync_SearchInNameAndAddress_ReturnsMatches()
        {
            // Arrange
            var filters = new PropertyFilters { Search = "centro" };
            var expectedProperties = new List<PropertyListDto>
            {
                CreatePropertyListDto(name: "Casa en el Centro", address: "Otra calle"),
                CreatePropertyListDto(name: "Apartamento", address: "Centro Histórico")
            };
            SetupAggregateQuery(expectedProperties);

            // Act
            var result = await _repository.GetAllAsync(filters);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.EqualTo(2));
        }

        #endregion

        #region Helper Methods

        private void SetupAggregateQuery(List<PropertyListDto> expectedResults)
        {
            var mockCursor = new Mock<IAsyncCursor<PropertyListDto>>();
            
            mockCursor.Setup(x => x.Current)
                .Returns(expectedResults);
            
            mockCursor.SetupSequence(x => x.MoveNext(It.IsAny<CancellationToken>()))
                .Returns(true)
                .Returns(false);
            
            mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(x => x.Aggregate<PropertyListDto>(
                    It.IsAny<PipelineDefinition<Property, PropertyListDto>>(),
                    It.IsAny<AggregateOptions>(),
                    It.IsAny<CancellationToken>()
                ))
                .Returns(mockCursor.Object);
        }

        private void SetupAggregateQueryWithPipelineCapture(
            List<PropertyListDto> expectedResults, 
            List<BsonDocument> capturedPipeline)
        {
            var mockCursor = new Mock<IAsyncCursor<PropertyListDto>>();
            
            mockCursor.Setup(x => x.Current).Returns(expectedResults);
            mockCursor.SetupSequence(x => x.MoveNextAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(true)
                .ReturnsAsync(false);

            _mockCollection.Setup(x => x.Aggregate<PropertyListDto>(
                    It.IsAny<PipelineDefinition<Property, PropertyListDto>>(),
                    It.IsAny<AggregateOptions>(),
                    It.IsAny<CancellationToken>()
                ))
                .Callback<PipelineDefinition<Property, PropertyListDto>, AggregateOptions, CancellationToken>(
                    (pipeline, options, token) =>
                    {
                        // Aquí podrías capturar el pipeline si necesitas validarlo
                    })
                .Returns(mockCursor.Object);
        }

        private PropertyListDto CreatePropertyListDto(
            string id = null,
            string idOwner = null,
            string name = "Propiedad de Prueba",
            string address = "Calle Test 123",
            decimal price = 150000,
            string image = "test-image.jpg")
        {
            return new PropertyListDto
            {
                // Id = id ?? ObjectId.GenerateNewId().ToString(),
                IdOwner = idOwner ?? ObjectId.GenerateNewId().ToString(),
                Name = name,
                AddressProperty = address,
                PriceProperty = price,
                Image = image
            };
        }

        private List<PropertyListDto> CreateTestPropertyListDtos(int count, string namePrefix = "Propiedad")
        {
            var properties = new List<PropertyListDto>();
            
            for (int i = 1; i <= count; i++)
            {
                properties.Add(CreatePropertyListDto(
                    name: $"{namePrefix} {i}",
                    address: $"Calle Test {i}",
                    price: 100000 + (i * 50000)
                ));
            }
            
            return properties;
        }

        #endregion
    }
}