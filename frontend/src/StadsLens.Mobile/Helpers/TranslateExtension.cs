using StadsLens.Mobile.Services;

namespace StadsLens.Mobile.Helpers;

// Gebruik in XAML: Text="{loc:Translate Login_Button}".
// Het is een binding op de LocalizationService, dus de tekst verandert mee wanneer de taal gewijzigd wordt.
[ContentProperty(nameof(Key))]
public class TranslateExtension : IMarkupExtension<BindingBase>
{
    public string Key { get; set; } = string.Empty;

    public BindingBase ProvideValue(IServiceProvider serviceProvider) =>
        new Binding($"[{Key}]", BindingMode.OneWay, source: LocalizationService.Instance);

    object IMarkupExtension.ProvideValue(IServiceProvider serviceProvider) => ProvideValue(serviceProvider);
}
