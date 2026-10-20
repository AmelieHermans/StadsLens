namespace StadsLens.Mobile.Constants;

public static class ApiSettings
{
    // Vul de URL van jullie Ngrok-tunnel in (ngrok http 5158) om op een echt toestel te testen.
    // Leeg laten = rechtstreeks naar de lokale API (dotnet run --launch-profile http).
    public const string NgrokUrl = "";

    private const int LocalPort = 5158;

    public static string BaseUrl =>
        !string.IsNullOrWhiteSpace(NgrokUrl)
            ? NgrokUrl.TrimEnd('/') + "/"
            // De Android-emulator bereikt de localhost van de pc via 10.0.2.2.
            : DeviceInfo.Platform == DevicePlatform.Android
                ? $"http://10.0.2.2:{LocalPort}/"
                : $"http://localhost:{LocalPort}/";
}
