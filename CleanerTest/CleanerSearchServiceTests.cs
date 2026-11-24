using Clean.Core.Entities;
using Clean.Service;

namespace CleanerTest
{
    public class CleanerSearchServiceTests
    {
        // The duration check runs before any repository is used, so no repositories are needed here.
        private static CleanerSearchService CreateService() =>
            new(null!, null!, null!, null!, TimeProvider.System);

        [Theory]
        [InlineData(24.5)]
        [InlineData(100)]
        [InlineData(1e300)]
        [InlineData(double.PositiveInfinity)]
        public async Task SearchAsync_rejects_an_absurd_duration_as_a_bad_request(double hours)
        {
            await Assert.ThrowsAsync<ArgumentException>(() =>
                CreateService().SearchAsync(new Customers(), new DateOnly(2030, 1, 1), new TimeOnly(8, 0), hours, null));
        }
    }
}
