using MapsterMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SimpleECommerceBackend.Api.DTOs.Roles;
using SimpleECommerceBackend.Application.UseCases.Roles.Create;
using SimpleECommerceBackend.Application.UseCases.Roles.Delete;
using SimpleECommerceBackend.Application.UseCases.Roles.GetAll;
using SimpleECommerceBackend.Application.UseCases.Roles.GetById;
using SimpleECommerceBackend.Application.UseCases.Roles.Update;

namespace SimpleECommerceBackend.Api.Controllers;

[ApiController]
[Route("api/v{version:apiVersion}/roles")]
[ApiVersion("1.0")]
[AutoConstructor]
public partial class RoleController : ControllerBase
{
    private readonly IMapper _mapper;
    private readonly IMediator _mediator;

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var result = await _mediator.Send(new GetAllRolesQuery());
        var response = result.Roles
            .Select(r => _mapper.Map<RoleResponse>(r));

        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var result = await _mediator.Send(new GetRoleByIdQuery(id));

        return Ok(_mapper.Map<RoleResponse>(result));
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreateRoleRequest dto)
    {
        var command = _mapper.Map<CreateRoleCommand>(dto);

        var result = await _mediator.Send(command);

        return CreatedAtAction(
            nameof(GetById),
            new { id = result.RoleId });
    }

    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(
        Guid id,
        [FromBody] UpdateRoleRequest dto
    )
    {
        var command = _mapper.Map<UpdateRoleCommand>((id, dto));
        var result = await _mediator.Send(command);

        return Ok(new { roleId = result.RoleId });
    }

    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var result = await _mediator.Send(new DeleteRoleCommand(id));

        return Ok(new { roleId = result.RoleId });
    }
}