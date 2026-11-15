using Clean.Service;
using Microsoft.Extensions.Logging.Abstractions;
using System.Globalization;
using System.Net;

namespace CleanerTest
{
    public class LocationServiceTests
    {
        private sealed class FakeHandler : HttpMessageHandler
        {
            private readonly Queue<HttpResponseMessage> _responses;
            public List<string> RequestedUrls { get; } = new();

            public FakeHandler(params (HttpStatusCode Status, string Body)[] responses)
            {
                _responses = new Queue<HttpResponseMessage>(responses.Select(r =>
                    new HttpResponseMessage(r.Status) { Content = new StringContent(r.Body) }));
            }

            protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
            {
                RequestedUrls.Add(request.RequestUri!.AbsoluteUri);
                return Task.FromResult(_responses.Dequeue());
            }
        }

        private static LocationService Create(FakeHandler handler) =>
            new(new HttpClient(handler) { BaseAddress = new Uri("https://test.local/") }, NullLogger<LocationService>.Instance);

        private const string Found = "[{\"lat\":\"32.0853\",\"lon\":\"34.7818\"}]";

        [Fact]
        public async Task Parses_coordinates_even_when_machine_culture_uses_comma()
        {
            var original = CultureInfo.CurrentCulture;
            CultureInfo.CurrentCulture = new CultureInfo("de-DE");
            try
            {
                var result = await Create(new FakeHandler((HttpStatusCode.OK, Found)))
                    .GetCoordinatesAsync("Tel Aviv", "Dizengoff", "");

                Assert.Equal(32.0853, result!.Value.lat, 4);
                Assert.Equal(34.7818, result.Value.lon, 4);
            }
            finally
            {
                CultureInfo.CurrentCulture = original;
            }
        }

        [Fact]
        public async Task Falls_back_to_zip_when_street_is_not_found()
        {
            var handler = new FakeHandler((HttpStatusCode.OK, "[]"), (HttpStatusCode.OK, Found));

            var result = await Create(handler).GetCoordinatesAsync("Tel Aviv", "Nowhere", "6100000");

            Assert.NotNull(result);
            Assert.Equal(2, handler.RequestedUrls.Count);
            Assert.Contains("street=", handler.RequestedUrls[0]);
            Assert.Contains("postalcode=6100000", handler.RequestedUrls[1]);
        }

        [Fact]
        public async Task Skips_street_search_when_city_or_street_is_empty()
        {
            var handler = new FakeHandler((HttpStatusCode.OK, Found));

            await Create(handler).GetCoordinatesAsync("", "", "6100000");

            Assert.Single(handler.RequestedUrls);
            Assert.Contains("postalcode=", handler.RequestedUrls[0]);
        }

        [Fact]
        public async Task Returns_null_when_nothing_is_found()
        {
            var handler = new FakeHandler((HttpStatusCode.OK, "[]"), (HttpStatusCode.OK, "[]"));

            Assert.Null(await Create(handler).GetCoordinatesAsync("Tel Aviv", "Nowhere", "0"));
        }

        [Fact]
        public async Task Returns_null_when_all_fields_are_empty()
        {
            var handler = new FakeHandler();

            Assert.Null(await Create(handler).GetCoordinatesAsync("", "", ""));
            Assert.Empty(handler.RequestedUrls);
        }

        [Fact]
        public async Task Returns_null_on_server_error()
        {
            Assert.Null(await Create(new FakeHandler((HttpStatusCode.InternalServerError, "")))
                .GetCoordinatesAsync("", "", "6100000"));
        }

        [Fact]
        public async Task Returns_null_on_broken_json()
        {
            Assert.Null(await Create(new FakeHandler((HttpStatusCode.OK, "not json")))
                .GetCoordinatesAsync("", "", "6100000"));
        }

        [Fact]
        public async Task Escapes_city_and_street_in_the_url()
        {
            var handler = new FakeHandler((HttpStatusCode.OK, Found));

            await Create(handler).GetCoordinatesAsync("תל אביב", "דיזנגוף & co", "");

            Assert.DoesNotContain(" ", handler.RequestedUrls[0]);
            Assert.DoesNotContain("& co", handler.RequestedUrls[0]);
        }

        [Fact]
        public void Distance_between_tel_aviv_and_jerusalem_is_about_54_km()
        {
            var service = Create(new FakeHandler());

            var km = service.CalculateDistance(32.0853, 34.7818, 31.7683, 35.2137);

            Assert.InRange(km, 52, 56);
        }

        [Fact]
        public void Distance_from_a_point_to_itself_is_zero()
        {
            Assert.Equal(0, Create(new FakeHandler()).CalculateDistance(32.0853, 34.7818, 32.0853, 34.7818), 6);
        }
    }
}
