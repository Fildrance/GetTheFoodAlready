using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json;

using NUnit.Framework;

namespace GetTheFoodAlready.DeliveryClubBridge.Tests.Integration
{
	public class DeliveryClubClientTests
	{
		private IDeliveryClubClient _client;
		[SetUp]
		public void Setup()
		{
			_client = new DeliveryClubClient(JsonSerializer.Create(), new LoggingHandler(new HttpClientHandler()));
		}

		[Test]
		public void GetDeliveryClubVendorsNearby_PassMoscowCoordinates_DoesntThrow()
		{
			//arrange
			var coordinatesLatitude = 55.867051m;
			var coordinatesLongitude = 37.594261m;
			//act
			//assert
			Assert.DoesNotThrowAsync(async () => await _client.GetDeliveryClubVendorsNearby(coordinatesLongitude, coordinatesLatitude, CancellationToken.None));
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_PassCertainMoscowCoordinates_ReturnProntoAltuf()
		{
			//arrange
			var coordinatesLatitude = 55.867051m;
			var coordinatesLongitude = 37.594261m;
			//act
			var result = await _client.GetDeliveryClubVendorsNearby(coordinatesLongitude, coordinatesLatitude, CancellationToken.None);
			//assert
			Assert.True(result.PagedList.Vendors.Any(x => x.Alias == "Altufjevskoje_shossje"));
		}
	}
}