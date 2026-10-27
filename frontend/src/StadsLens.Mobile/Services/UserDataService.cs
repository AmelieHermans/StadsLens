using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Services;

public class UserDataService : ApiService<User>, IUserDataService
{
    public UserDataService(HttpClient httpClient) : base(httpClient, "api/users")
    {
    }
}
