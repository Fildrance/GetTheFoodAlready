using System;
using System.Collections.Generic;

using GetTheFoodAlready.Api.FoodAgregators.Responses;

using MediatR;

namespace GetTheFoodAlready.Api.FoodAgregators.Requests
{
	/// <summary> Request for list of acceptable food items. </summary>
	public class FoodInfoGetRequest : IRequest<FoodInfoGetResponse>
	{
		#region [c-tor]
		public FoodInfoGetRequest(int id, IReadOnlyCollection<string> acceptedCategories)
		{
			Id = id;
			AcceptedCategories = acceptedCategories ?? throw new ArgumentNullException(nameof(acceptedCategories));
		}
		#endregion

		#region [Public]
		#region [Public properties]
		public int Id { get; }

		public IReadOnlyCollection<string> AcceptedCategories { get; }
		#endregion
		#endregion
	}
}