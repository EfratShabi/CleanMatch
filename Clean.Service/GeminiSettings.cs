namespace Clean.Service
{
    public record GeminiSettings(string ApiKey, string Model, TimeSpan RetryBaseDelay);
}
