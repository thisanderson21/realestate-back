using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Application.DTOS;
using Application.Interfaces;
using Application.Queries.GetAllProperties;
using Moq;
using NUnit.Framework;

namespace Application.Tests.Queries
{
  [TestFixture]
  public class GetAllPropertiesHandlerTests
  {
    private Mock<IPropertyRepository> _mockRepository;
    private GetAllPropertiesHandler _handler;

    [SetUp]
    public void Setup()
    {
      _mockRepository = new Mock<IPropertyRepository>();
      _handler = new GetAllPropertiesHandler(_mockRepository.Object);
    }

    [Test]
    public async Task Handle_ShouldReturnListOfProperties_WhenRepositoryReturnsData()
    {
      // Arrange
      var filters = new PropertyFilters();
      var expectedProperties = new List<PropertyListDto>
            {
                new PropertyListDto
                {
                    _id = "1",
                    IdOwner = "Owner1",
                    Name = "Casa bonita",
                    AddressProperty = "Calle 123",
                    PriceProperty = 1000000,
                    Image = "image1.png"
                },
                new PropertyListDto
                {
                    _id = "2",
                    IdOwner = "Owner2",
                    Name = "Apartamento moderno",
                    AddressProperty = "Carrera 45",
                    PriceProperty = 2000000,
                    Image = "image2.png"
                }
            };

      _mockRepository
          .Setup(r => r.GetAllAsync(filters))
          .ReturnsAsync(expectedProperties);

      var query = new GetAllPropertiesQuery(filters);

      // Act
      var result = await _handler.Handle(query, CancellationToken.None);

      // Assert
      Assert.That(result, Is.Not.Null);
      Assert.That(expectedProperties.Count, Is.EqualTo(result.Count));
      Assert.That(expectedProperties[0].Name, Is.EqualTo(result[0].Name));
      _mockRepository.Verify(r => r.GetAllAsync(filters), Times.Once);
    }

    [Test]
    public async Task Handle_ShouldReturnEmptyList_WhenRepositoryReturnsEmpty()
    {
      // Arrange
      var filters = new PropertyFilters{
          Search = "No search",
          MinPrice = 1000000,
          MaxPrice = 40000000
      };;

      
      _mockRepository
          .Setup(r => r.GetAllAsync(filters))
          .ReturnsAsync(new List<PropertyListDto>());

      var query = new GetAllPropertiesQuery(filters);

      // Act
      var result = await _handler.Handle(query, CancellationToken.None);

      // Assert
      Assert.That(result, Is.Not.Null);
      Assert.That(result, Is.Empty);
      _mockRepository.Verify(r => r.GetAllAsync(filters), Times.Once);
    }
  }
}
