using AutoMapper;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Security;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Users;
using StadsLens.Domain.Entities;

namespace StadsLens.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IMapper mapper)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _mapper = mapper;
    }

    public async Task<IEnumerable<UserResponse>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<UserResponse>>(users);
    }

    public async Task<UserResponse?> GetByIdAsync(int id)
    {
        var user = await _userRepository.GetByIdAsync(id);
        return user is null ? null : _mapper.Map<UserResponse>(user);
    }

    public async Task<UserResponse?> RegisterAsync(RegisterRequest request)
    {
        var name = request.Name.Trim();
        if (await _userRepository.GetByNameAsync(name) is not null)
            return null;

        var user = new User
        {
            Name = name,
            PasswordHash = _passwordHasher.Hash(request.Password),
            PreferredLanguage = Languages.IsSupported(request.PreferredLanguage) ? request.PreferredLanguage : Languages.Default
        };

        var created = await _userRepository.AddAsync(user);
        return _mapper.Map<UserResponse>(created);
    }

    public async Task<UserResponse?> LoginAsync(LoginRequest request)
    {
        var user = await _userRepository.GetByNameAsync(request.Name.Trim());
        if (user is null || !_passwordHasher.Verify(request.Password, user.PasswordHash))
            return null;

        return _mapper.Map<UserResponse>(user);
    }
}
