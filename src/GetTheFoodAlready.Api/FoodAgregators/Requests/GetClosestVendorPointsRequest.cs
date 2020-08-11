using GetTheFoodAlready.Api.FoodAgregators.Responses;

using MediatR;

namespace GetTheFoodAlready.Api.FoodAgregators.Requests
{
	/// <summary>
	/// Request data for getting closest vendor points. 
	/// </summary>
	public sealed class GetClosestVendorPointsRequest : IRequest<GetClosestVendorPointsResponse>
	{
		public decimal Longitude { get; set; }
		public decimal Latitude { get; set; }
	}
}