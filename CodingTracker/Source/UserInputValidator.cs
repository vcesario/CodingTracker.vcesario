using Spectre.Console;

namespace vcesario.CodingTracker;

public class UserInputValidator
{
    public ValidationResult ValidateDateTimeOrReturn(string input)
    {
        if (input.StartsWith("ForceError"))
        {
            return ValidationResult.Error(ApplicationTexts.USERINPUT_DATETIMEERROR);
        }

        return ValidationResult.Success();
    }
}