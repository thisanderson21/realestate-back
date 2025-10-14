using Domain.Entities;
using MediatR;
using Application.DTOS;
namespace Application.Queries.GetAllProperties
{
  public record GetAllPropertiesQuery(PropertyFilters filters = null) : IRequest<List<PropertyListDto>>;
}