using CommunityToolkit.Mvvm.Messaging.Messages;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Messages;

public class LandmarkRecognizedMessage : ValueChangedMessage<Landmark>
{
    public LandmarkRecognizedMessage(Landmark landmark) : base(landmark)
    {
    }
}
