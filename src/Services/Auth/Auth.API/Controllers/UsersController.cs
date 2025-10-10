using Auth.Application.Users.Commands;
using Auth.Application.Users.Notifications;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Auth.API.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController(IMediator mediator) : ControllerBase
{
    [HttpGet("@me")]
    [Authorize]
    [EndpointSummary("Get current user")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetCurrentUser(CancellationToken cancellationToken)
    {
        return Ok();
    }

    [HttpPost]
    [EndpointSummary("Create a new user")]
    [Consumes("application/json")]
    [Produces("application/json")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    public async Task<IActionResult> CreateUser([FromBody] CreateUserCommand command, CancellationToken cancellationToken)
    {
        var userId = await mediator.Send(command, cancellationToken);
        await mediator.Publish(new UserCreatedNotification { Id = userId }, cancellationToken);
        return CreatedAtAction(nameof(CreateUser), new { id = userId }, null);
    }
}