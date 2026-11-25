using Clean.Core.Llm;
using System.Net;
using System.Text;

namespace Clean.Service
{
    public class GeminiClient : ILlmClient
    {
        private readonly HttpClient _http;
        private readonly GeminiSettings _settings;

        private const int MaxAttempts = 3;

        public GeminiClient(HttpClient http, GeminiSettings settings)
        {
            _http = http;
            _settings = settings;
        }

        public async Task<LlmReply> SendAsync(
            IReadOnlyList<LlmMessage> conversation, IReadOnlyList<LlmTool>? tools = null, CancellationToken ct = default)
        {
            for (var attempt = 1; ; attempt++)
            {
                try
                {
                    return await SendOnceAsync(conversation, tools, ct);
                }
                catch (LlmException error) when (error.IsTemporary && attempt < MaxAttempts)
                {
                    var delay = _settings.RetryBaseDelay * Math.Pow(2, attempt - 1);
                    await Task.Delay(delay, ct);
                }
            }
        }

        private async Task<LlmReply> SendOnceAsync(
            IReadOnlyList<LlmMessage> conversation, IReadOnlyList<LlmTool>? tools, CancellationToken ct)
        {
            if (string.IsNullOrWhiteSpace(_settings.ApiKey))
            {
                throw new LlmException("Gemini API key is not configured.", isTemporary: false);
            }

            var body = GeminiJson.BuildRequest(conversation, tools);

            using var request = new HttpRequestMessage(HttpMethod.Post, $"models/{_settings.Model}:generateContent")
            {
                Content = new StringContent(body, Encoding.UTF8, "application/json")
            };
            request.Headers.Add("x-goog-api-key", _settings.ApiKey);

            try
            {
                using var response = await _http.SendAsync(request, ct);
                var responseBody = await response.Content.ReadAsStringAsync(ct);

                if (response.IsSuccessStatusCode)
                {
                    return GeminiJson.ParseReply(responseBody);
                }

                var isTemporary = response.StatusCode == HttpStatusCode.RequestTimeout
                    || response.StatusCode == HttpStatusCode.TooManyRequests
                    || (int)response.StatusCode >= 500;

                throw new LlmException(
                    $"Gemini returned {(int)response.StatusCode}: {responseBody}",
                    isTemporary);
            }
            catch (HttpRequestException error)
            {
                throw new LlmException("Could not reach Gemini.", isTemporary: true, error);
            }
            catch (TaskCanceledException error) when (!ct.IsCancellationRequested)
            {
                throw new LlmException("Gemini did not answer in time.", isTemporary: true, error);
            }
        }
    }
}
