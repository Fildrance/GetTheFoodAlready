using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using GetTheFoodAlready.Api.Support;
using GetTheFoodAlready.DeliveryClubBridge.Client;

using NUnit.Framework;

namespace GetTheFoodAlready.DeliveryClubBridge.Tests.Integration
{
	[ExcludeFromCodeCoverage, TestFixture, Category("Integration")]
	public class DeliveryClubClientTests
	{
		private const int ShokoladnicaVendorPoint = 28022;
		private const string CoordinatesLatitude = "55.867051";
		private const string CoordinatesLongitude = "37.594261";
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
			await _client.Login(CancellationToken.None);
			//act
			//assert
			Assert.DoesNotThrowAsync(async () => await _client.GetDeliveryClubVendorsNearby(CoordinatesLongitude, CoordinatesLatitude, CancellationToken.None));
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_InvalidCoordinates_Throws()
		{
			//arrange
			var coordinatesLongitude = "someStuff";
			await _client.Login(CancellationToken.None);
			//act
			//assert
			Assert.ThrowsAsync<HttpRequestException>(async () => await _client.GetDeliveryClubVendorsNearby(coordinatesLongitude, CoordinatesLatitude, CancellationToken.None));
		}

		[Test]
		public async Task GetDeliveryClubVendorsNearby_PassCertainMoscowCoordinates_ReturnProntoAltuf()
		{
			//arrange
			await _client.Login(CancellationToken.None);
			//act
			var result = await _client.GetDeliveryClubVendorsNearby(CoordinatesLongitude, CoordinatesLatitude, CancellationToken.None);
			//assert
			Assert.True(result.PagedList.Vendors.Any(x => x.Alias == "Altufjevskoje_shossje"));
		}

		// Login method cannot be tested apart due to no visible side effects (mostly bcoz HttpClient is not abstraction).
		[Test]
		public void GetDeliveryClubVendorsNearby_NoLogin_Throws()
		{
			//arrange
			//act
			var r = Assert.ThrowsAsync<HttpRequestException>(async () => await _client.GetDeliveryClubVendorsNearby(CoordinatesLongitude, CoordinatesLatitude, CancellationToken.None));
			//assert
			Assert.True(r.Message.Contains("401"));
		}

		[Test]
		public async Task GetFoodInfo_PassCertainVendorPoint_ReturnsHotChocolate()
		{
			//arrange
			await _client.Login(CancellationToken.None);
			//act
			var result = await _client.GetFoodInfo(ShokoladnicaVendorPoint, CancellationToken.None);
			//assert
			CollectionAssert.IsNotEmpty(result.Products);
			Assert.True(result.Products.Any(x => x.Name == "Шоколад горький"));
		}

		[Test]
		public void GetFoodInfo_NoLogin_Throws()
		{
			//arrange
			//act
			var r = Assert.ThrowsAsync<HttpRequestException>(async () => await _client.GetFoodInfo(ShokoladnicaVendorPoint, CancellationToken.None));
			//assert
			Assert.True(r.Message.Contains("401"));
		}
	}
}