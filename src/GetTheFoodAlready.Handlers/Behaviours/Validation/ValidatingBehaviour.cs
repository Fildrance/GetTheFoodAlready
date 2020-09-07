using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using FluentValidation;
using FluentValidation.Results;

using MediatR;

namespace GetTheFoodAlready.Handlers.Behaviours.Validation
{
	/// <summary> Behaviour that automatically validates every incoming request. </summary>
	public class ValidatingBehaviour<TRequest, TResponse> : IPipelineBehavior<TRequest, TResponse>
	{
		#region [Fields]
		private readonly IValidatorProvider _validatorProvider;
		#endregion

		#region [c-tor]
		public ValidatingBehaviour(IValidatorProvider validatorProvider)
		{
			_validatorProvider = validatorProvider ?? throw new ArgumentNullException(nameof(validatorProvider));
		}
		#endregion

		#region IPipelineBehavior<TRequest,TResponse> implementation
		public Task<TResponse> Handle(TRequest request, CancellationToken cancellationToken, RequestHandlerDelegate<TResponse> next)
		{
			var validators = _validatorProvider.GetValidators<TRequest>();
			var errors = new List<ValidationFailure>();
			foreach (var validator in validators)
			{
				var validationResult = validator.Validate(new ValidationContext<TRequest>(request));
				if (!validationResult.IsValid)
				{
					errors.AddRange(validationResult.Errors);
				}
			}

			if (errors.Any())
			{
				throw new ValidationException(errors);
			}

			return next();
		}
		#endregion
	}
}