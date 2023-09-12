using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using VL.Health.API.Validators.Interfaces;
using VL.Health.Domain.Enums;

namespace VL.Health.API.Validators
{
	public abstract class AbstractBaseValidator<T> : AbstractValidator<T>, ICustomValidator<T>
	{
		protected ActionType _actionType;
		private Action _addValidations { get; set; }

		public void AddValidations(Action addValidations)
		{
			_addValidations = addValidations;
		}

		public List<string> Errors { get; private set; }

		public bool IsValid(T @object, ActionType actionType)
		{
			_actionType = actionType;

			_addValidations();

			var validationResult = base.Validate(@object);

			Errors = validationResult.Errors.Select(x => x.ErrorMessage).ToList();

			return validationResult.IsValid;
		}

		#region Helper Methods

		public bool HasDuplicateItems<TInput, TKey>(List<TInput> list, Func<TInput, TKey> compoundKey)
		{
			var duplicates = list
				.GroupBy(compoundKey)
				.Where(x => x.Count() > 1)
				.Select(x => x.Key);

			return duplicates.Any();
		}

		#endregion
	}
}
