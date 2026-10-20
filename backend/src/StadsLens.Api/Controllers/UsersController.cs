using Microsoft.AspNetCore.Mvc;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Users;
using StadsLens.Contracts.Visits;

namespace StadsLens.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly IVisitService _visitService;

    public UsersController(IUserService userService, IVisitService visitService)
    {
        _userService = userService;
        _visitService = visitService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<UserResponse>>> GetAll()
    {
        return Ok(await _userService.GetAllAsync());
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<UserResponse>> GetById(int id)
    {
        var user = await _userService.GetByIdAsync(id);
        return user is null ? NotFound() : Ok(user);
    }

    [HttpPost]
    public async Task<ActionResult<UserResponse>> Create(CreateUserRequest request)
    {
        var user = await _userService.CreateAsync(request);
        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    [HttpGet("{id:int}/visits")]
    public async Task<ActionResult<IEnumerable<VisitResponse>>> GetVisits(int id)
    {
        return Ok(await _visitService.GetByUserIdAsync(id));
    }
}
