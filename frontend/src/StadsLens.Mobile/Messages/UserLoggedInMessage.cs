using CommunityToolkit.Mvvm.Messaging.Messages;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Messages;

public class UserLoggedInMessage : ValueChangedMessage<User>
{
    public UserLoggedInMessage(User user) : base(user)
    {
    }
}
