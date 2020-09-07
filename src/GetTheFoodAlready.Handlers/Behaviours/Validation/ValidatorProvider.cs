using System;
using System.Collections.Generic;
using System.Linq;

using FluentValidation;

namespace GetTheFoodAlready.Handlers.Behaviours.Validation
{
	/// <summary> Default validator provider implementation. </summary>
	public class ValidatorProvider : IValidatorProvider
	{
		#region [Fields]
		private readonly IEnumerable<IValidator> _validators;
		#endregion

		#region [c-tor]
		public ValidatorProvider(IEnumerable<IValidator> validators)
		{
			_validators = validators ?? throw new ArgumentNullException(nameof(validators));
		}
		#endregion

		#region [Public]
		#region [Public methods]
		public IEnumerable<IValidator> GetValidators<TType>()
		{
			return _validators.Where(validator => validator.CanValidateInstancesOfType(typeof(TType)));
		}
		#endregion
		#endregion
	}
}
