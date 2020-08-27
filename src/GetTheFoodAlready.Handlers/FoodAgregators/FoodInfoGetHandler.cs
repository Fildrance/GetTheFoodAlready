using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using AutoMapper;

using GetTheFoodAlready.Api.FoodAgregators.Requests;
using GetTheFoodAlready.Api.FoodAgregators.Responses;
using GetTheFoodAlready.DeliveryClubBridge;

using MediatR;

using NLog;

namespace GetTheFoodAlready.Handlers.FoodAgregators
{
	/// <summary> Handles getting food information of certain vendor point. </summary>
	public class FoodInfoGetHandler : IRequestHandler<FoodInfoGetRequest, FoodInfoGetResponse>
	{
		#region [Static fields]
		private static readonly ILogger Logger = LogManager.GetLogger(typeof(FoodInfoGetHandler).FullName);
		#endregion

		#region [Fields]
		private readonly IDeliveryClubClient _client;
		private readonly IMapper _mapper;
		#endregion

		#region [c-tor]
		public FoodInfoGetHandler(
			IDeliveryClubClient client,
			IMapper mapper
		)
		{
			_client = client ?? throw new ArgumentNullException(nameof(client));
			_mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
		}
		#endregion

		#region IRequestHandler<FoodInfoGetRequest,FoodInfoGetResponse> implementation
		public async Task<FoodInfoGetResponse> Handle(FoodInfoGetRequest request, CancellationToken cancellationToken)
		{
			var foodInfoResponse = await _client.GetFoodInfo(request.Id, cancellationToken);
			Logger.Trace($"Acquired {foodInfoResponse.Menu.Count} menu items and {foodInfoResponse.Products.Count} products.");
			var result = foodInfoResponse.Products
				.Select(x => {
					var menuCategories = foodInfoResponse.Menu;
					var id = x.Id.Primary;
					var belongsToMenuCategory = menuCategories.FirstOrDefault(c => c.ProductIds.Contains(id));
					
					return new
					{
						Item = x,
						Category = belongsToMenuCategory
					};
				}).Where(x => {
					if (x.Category == null)
					{
						return true;
					}
					return !request.AcceptedCategories.Any(ac => ac.Contains(x.Category.Name));
				}).Select(x => {
					var mapped = _mapper.Map<FoodInfo>(x.Item);
					mapped.CategoryName = x.Category?.Name;
					return mapped;
				}).ToArray();

			Logger.Debug(() =>
			{
				var message = $"Acquired {result.Length} food items for vendor with id '{request.Id}'";
				if (request.AcceptedCategories.Count > 0)
				{
					message = message + $", while excepting food with category {string.Join(",", request.AcceptedCategories)}";
				} 
				return message + ".";
			});

			return new FoodInfoGetResponse(result);
		}
		#endregion
	}

}