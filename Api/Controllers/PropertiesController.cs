
using Application.Queries.GetAllProperties;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
  [ApiController]
  [Route("api/[controller]")]
  public class PropertiesController : ControllerBase
  {
    private readonly IMediator _mediator;

    public PropertiesController(IMediator mediator)
    {
      _mediator = mediator;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(
      [FromQuery] string? search,
      [FromQuery] decimal? minPrice,
      [FromQuery] decimal? maxPrice
    )
    {
       var filters = new PropertyFilters
      {
          Search = search,
          MinPrice = minPrice,
          MaxPrice = maxPrice
      };
      var query = new GetAllPropertiesQuery(filters);
      var result = await _mediator.Send(query);
      return Ok(result);
    }
  }
}