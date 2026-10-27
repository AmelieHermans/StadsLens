using CommunityToolkit.Mvvm.Messaging.Messages;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Messages;

// Verstuurd wanneer de gebruiker een bezienswaardigheid kiest (bv. in de lijst van een route).
public class LandmarkSelectedMessage(Landmark landmark) : ValueChangedMessage<Landmark>(landmark)
{
}
