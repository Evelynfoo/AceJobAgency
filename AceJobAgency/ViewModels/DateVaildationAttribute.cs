using System.ComponentModel.DataAnnotations;

namespace AceJobAgency.ViewModels
{
	public class DateVaildationAttribute: ValidationAttribute
	{
		int _minimumAge;

		public DateVaildationAttribute(int minimumAge)
		{
			_minimumAge = minimumAge;
		}

		public override bool IsValid(object? value)
		{
			DateTime date;
			if (DateTime.TryParse(value.ToString(), out date))
			{
				return date.AddYears(_minimumAge) < DateTime.Now;
			}
			return false;
		}
	}
}
