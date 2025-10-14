using Application.Interfaces;
using Domain.Entities;
using MediatR;
using Application.DTOS;

namespace Application.Queries.GetAllProperties
{
  public class GetAllPropertiesHandler : IRequestHandler<GetAllPropertiesQuery, List<PropertyListDto>>
  {
    private readonly IPropertyRepository _repository;

    public GetAllPropertiesHandler(IPropertyRepository repository)
    {
      _repository = repository;
    }

    public async Task<List<PropertyListDto>> Handle(GetAllPropertiesQuery request, CancellationToken cancellationToken)
    {

      var properties = await _repository.GetAllAsync(request.filters);


      return properties;
    }
  }
}
