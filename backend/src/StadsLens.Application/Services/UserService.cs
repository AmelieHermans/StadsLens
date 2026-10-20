using AutoMapper;
using StadsLens.Application.Interfaces.Repositories;
using StadsLens.Application.Interfaces.Services;
using StadsLens.Contracts.Users;
using StadsLens.Domain.Entities;

namespace StadsLens.Application.Services;

public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    public UserService(IUserRepository userRepository, IMapper mapper)
    {
        _userRepository = userRepository;
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

    public async Task<UserResponse> CreateAsync(CreateUserRequest request)
    {
        var user = _mapper.Map<User>(request);
        var created = await _userRepository.AddAsync(user);
        return _mapper.Map<UserResponse>(created);
    }
}
