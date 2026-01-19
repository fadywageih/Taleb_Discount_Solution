using Microsoft.AspNetCore.Mvc;
using Shared.ErrorModels;
using System.Net;

namespace Taleb_Discount.Factories
{
    public class ApiResponseFactory
    {
        public static IActionResult CustomValidationErrorResponse(ActionContext context)
        {
            var errors = context.ModelState
                .Where(e => e.Value.Errors.Any()).
                Select(error => new ValidationError
                {
                    Field = error.Key,
                    Errors
                = error.Value.Errors.Select(e => e.ErrorMessage)
                });
            var response = new ValidationErrorResponse
            {
                StatusCode = (int)HttpStatusCode.BadRequest,
                ErrorMessage = "One or more validation errors occurred.",
                Errors = errors
            };
            return new BadRequestObjectResult(response);
        }

    }
}
