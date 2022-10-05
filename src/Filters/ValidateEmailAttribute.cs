using shopping_bag.Config;
using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace shopping_bag.Filters
{
    public class ValidateEmailAttribute : ValidationAttribute
    {
        public override string FormatErrorMessage(string name) =>
            string.Format(CultureInfo.CurrentCulture, ErrorMessageString, name);

        public override bool IsValid(object? value)
        {
            var emailChecker = new EmailAddressAttribute();
            var isEmailValid = emailChecker.IsValid(value);

            if (!isEmailValid || value == null)
            {
                return false;
            }

            return StaticConfig.AllowedEmailDomain.Any(it => value.ToString().Trim().EndsWith(it));
        }
    }
}
