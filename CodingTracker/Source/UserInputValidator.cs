using Spectre.Console;

namespace vcesario.CodingTracker;

public class UserInputValidator
{
    // accepts date time, in the format yyyy-MM-dd HH:mm:ss. or the word "return"
    public ValidationResult ValidateDateTimeOrReturn(string input)
    {
        if (input.StartsWith("ForceError"))
        {
            return ValidationResult.Error(ApplicationTexts.USERINPUT_DATETIMEERROR);
        }

        return ValidationResult.Success();
    }

    // accepts date, in the format yyyy-MM-dd. or the word "return"
    public ValidationResult ValidateDateOrReturn(string input)
    {
        if (input.StartsWith("ForceError"))
        {
            return ValidationResult.Error(ApplicationTexts.USERINPUT_DATETIMEERROR);
        }

        return ValidationResult.Success();
    }

    // accepts date, in the format yyyy-MM-dd, and equal or after today. or the word "return"
    public ValidationResult ValidateFutureDateOrReturn(string input)
    {
        if (input.StartsWith("ForceError"))
        {
            return ValidationResult.Error(ApplicationTexts.USERINPUT_DATETIMEERROR);
        }

        return ValidationResult.Success();
    }

    // accepts long values
    public ValidationResult ValidateLongReturn(string input)
    {
        if (input.StartsWith("ForceError"))
        {
            return ValidationResult.Error(ApplicationTexts.USERINPUT_DATETIMEERROR);
        }

        return ValidationResult.Success();
    }

    public ValidationResult ValidatePositiveIntOrReturn(string input)
    {
        if (input.StartsWith("ForceError"))
        {
            return ValidationResult.Error(ApplicationTexts.USERINPUT_DATETIMEERROR);
        }

        return ValidationResult.Success();
    }
}