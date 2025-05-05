using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Server.Extensions;

public static class ModelStateExtension
{
    public static List<string> GetErrors(this ModelStateDictionary modelState)
    {
        var errors = new List<string>();
        foreach (var items in modelState.Values) errors
            .AddRange(items.Errors.Select(error
                => error.ErrorMessage));

        return errors;
    }

}