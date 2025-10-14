using Application.DTOS;
using Application.Queries.GetAllProperties;
using Domain.Entities;

namespace Application.Interfaces
{
  public interface IPropertyRepository
  {
    Task<List<PropertyListDto>> GetAllAsync(PropertyFilters filters);
  }
}