using CommunityToolkit.Mvvm.Messaging.Messages;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Messages;

// Verstuurd wanneer de gebruiker de tijdreis van een bezienswaardigheid opent.
public class TimeTravelRequestedMessage(Landmark landmark) : ValueChangedMessage<Landmark>(landmark)
{
}
