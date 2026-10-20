using CommunityToolkit.Mvvm.Messaging.Messages;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Messages;

public class VisitRegisteredMessage : ValueChangedMessage<Visit>
{
    public VisitRegisteredMessage(Visit visit) : base(visit)
    {
    }
}
