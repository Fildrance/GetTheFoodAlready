using System.Collections.Generic;

using FluentValidation;

namespace GetTheFoodAlready.Handlers.Behaviours.Validation
{
	/// <summary> Provider of validators. </summary>
	public interface IValidatorProvider
	{
		/// <summary>
		/// Provides enumeration of validators, that can validate provided parameter type.
		/// </summary>
		/// <typeparam name="TType">Type to be validated.</typeparam>
		/// <returns>Enumeration of validators.</returns>
		IEnumerable<IValidator> GetValidators<TType>();
	}
}