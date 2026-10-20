using CommunityToolkit.Mvvm.Messaging.Messages;

namespace StadsLens.Mobile.Messages;

// Value = taalcode ("nl" of "en")
public class LanguageChangedMessage : ValueChangedMessage<string>
{
    public LanguageChangedMessage(string languageCode) : base(languageCode)
    {
    }
}
