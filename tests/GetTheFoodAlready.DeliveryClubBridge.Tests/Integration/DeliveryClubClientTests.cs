using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.DeliveryClubBridge.Client;

using NUnit.Framework;

namespace GetTheFoodAlready.DeliveryClubBridge.Tests.Integration
{
	public class DeliveryClubClientTests
	{
		private IDeliveryClubClient _client;
		[SetUp]
		public void Setup()
		{
			var deliveryClubClientSettings = new DeliveryClubClientSettings("https://api.delivery-club.ru", "api1.2");

			_client = new DeliveryClubClient(nested => new LoggingHttpHandler(nested), deliveryClubClientSettings);
			
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_PassMoscowCoordinates_DoesntThrow()
		{
			//arrange
			var coordinatesLatitude = "55.867051m";
			var coordinatesLongitude = "37.594261m";
			await _client.Login(CancellationToken.None);
			//act
			//assert
			Assert.DoesNotThrowAsync(async () => await _client.GetDeliveryClubVendorsNearby(coordinatesLongitude, coordinatesLatitude, CancellationToken.None));
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_PassCertainMoscowCoordinates_ReturnProntoAltuf()
		{
			//arrange
			var coordinatesLatitude = "55.867051m";
			var coordinatesLongitude = "37.594261m";
			await _client.Login(CancellationToken.None);
			//act
			var result = await _client.GetDeliveryClubVendorsNearby(coordinatesLongitude, coordinatesLatitude, CancellationToken.None);
			//assert
			Assert.True(result.PagedList.Vendors.Any(x => x.Alias == "Altufjevskoje_shossje"));
		}
	}
}