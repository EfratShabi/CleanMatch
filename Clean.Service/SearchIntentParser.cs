using Clean.Core.Models;
using System.Globalization;
using System.Text.Json;

namespace Clean.Service
{
    public static class SearchIntentParser
    {
        public static SearchIntent? Parse(string modelText)
        {
            var json = ExtractJsonObject(modelText);
            if (json == null)
            {
                return null;
            }

            try
            {
                using var document = JsonDocument.Parse(json);
                var root = document.RootElement;

                if (TryReadDate(root, out var date)
                    && TryReadStart(root, out var start)
                    && TryReadHours(root, out var hours)
                    && TryReadMaxPrice(root, out var maxPrice))
                {
                    return new SearchIntent(date, start, hours, maxPrice);
                }

                return null;
            }
            catch (JsonException)
            {
                return null;
            }
        }

        private static string? ExtractJsonObject(string modelText)
        {
            var first = modelText.IndexOf('{');
            var last = modelText.LastIndexOf('}');
            return first >= 0 && last > first ? modelText[first..(last + 1)] : null;
        }

        private static bool TryReadDate(JsonElement root, out DateOnly? value)
        {
            value = null;
            if (!root.TryGetProperty("date", out var element) || element.ValueKind == JsonValueKind.Null)
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.String
                && DateOnly.TryParseExact(element.GetString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date))
            {
                value = date;
                return true;
            }

            return false;
        }

        private static bool TryReadStart(JsonElement root, out TimeOnly? value)
        {
            value = null;
            if (!root.TryGetProperty("start", out var element) || element.ValueKind == JsonValueKind.Null)
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.String
                && TimeOnly.TryParseExact(element.GetString(), "HH:mm", CultureInfo.InvariantCulture, DateTimeStyles.None, out var start))
            {
                value = start;
                return true;
            }

            return false;
        }

        private static bool TryReadHours(JsonElement root, out double? value)
        {
            value = null;
            if (!root.TryGetProperty("hours", out var element) || element.ValueKind == JsonValueKind.Null)
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.Number && element.TryGetDouble(out var hours) && hours > 0)
            {
                value = hours;
                return true;
            }

            return false;
        }

        private static bool TryReadMaxPrice(JsonElement root, out int? value)
        {
            value = null;
            if (!root.TryGetProperty("maxPrice", out var element) || element.ValueKind == JsonValueKind.Null)
            {
                return true;
            }

            if (element.ValueKind == JsonValueKind.Number && element.TryGetInt32(out var price) && price > 0)
            {
                value = price;
                return true;
            }

            return false;
        }
    }
}
