using Microsoft.AspNetCore.Mvc;
using StadsLens.Application;
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

    // Nieuw account. 409 als de naam al bestaat.
    [HttpPost("register")]
    public async Task<ActionResult<UserResponse>> Register(RegisterRequest request)
    {
        var user = await _userService.RegisterAsync(request);
        return user is null
            ? Conflict(new { message = $"De naam '{request.Name.Trim()}' is al in gebruik." })
            : CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    // Bestaand account. 401 bij een onbekende naam of een fout wachtwoord (zonder te zeggen welk van de twee).
    [HttpPost("login")]
    public async Task<ActionResult<UserResponse>> Login(LoginRequest request)
    {
        var user = await _userService.LoginAsync(request);
        return user is null
            ? Unauthorized(new { message = "Naam of wachtwoord klopt niet." })
            : Ok(user);
    }

    // De namen van de landmarks in de taal van de app (Accept-Language-header of ?language=).
    [HttpGet("{id:int}/visits")]
    public async Task<ActionResult<IEnumerable<VisitResponse>>> GetVisits(int id, [FromQuery] string? language, CancellationToken cancellationToken)
    {
        language = RequestLanguage.Resolve(Request, language);
        if (!Languages.IsSupported(language))
            language = Languages.Default;

        return Ok(await _visitService.GetByUserIdAsync(id, language, cancellationToken));
    }
}
