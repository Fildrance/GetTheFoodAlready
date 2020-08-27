using GetTheFoodAlready.Api.FoodAgregators.Responses;

using MediatR;

namespace GetTheFoodAlready.Api.FoodAgregators.Requests
{
	/// <summary>
	/// Request data for getting closest vendor points. 
	/// </summary>
	public sealed class ClosestVendorPointsGetRequest : IRequest<ClosestVendorPointsGetResponse>
	{
		public string Longitude { get; set; }
		public string Latitude { get; set; }
	}
}