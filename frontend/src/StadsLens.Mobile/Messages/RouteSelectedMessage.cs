using CommunityToolkit.Mvvm.Messaging.Messages;
using StadsLens.Mobile.Models;

namespace StadsLens.Mobile.Messages;

// Verstuurd wanneer de gebruiker een wandelroute kiest; de detail- en kaartpagina luisteren hiernaar.
public class RouteSelectedMessage(Route route) : ValueChangedMessage<Route>(route)
{
}
