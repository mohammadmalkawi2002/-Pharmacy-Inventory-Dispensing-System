using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PharmacyInventoryDispensingSystem.Domain.Common.Results;
using PharmacyInventoryDispensingSystem.WebApi.Contracts.ApiResponse;

namespace PharmacyInventoryDispensingSystem.WebApi.Controllers
{
    [ApiController]
    public abstract class ApiController : ControllerBase
    {
        protected ActionResult Problem(List<Error> errors)
        {
            if (errors.Count is 0)
            {
                return Problem();
            }

            if (errors.All(error => error.Type == ErrorKind.Validation))
            {
                return ValidationProblem(errors);
            }

            return Problem(errors[0]);
        }

        private ObjectResult Problem(Error error)
        {
            var statusCode = error.Type switch
            {
                ErrorKind.Conflict => StatusCodes.Status409Conflict,
                ErrorKind.Validation => StatusCodes.Status400BadRequest,
                ErrorKind.NotFound => StatusCodes.Status404NotFound,
                ErrorKind.Unauthorized => StatusCodes.Status401Unauthorized,
                ErrorKind.Forbidden => StatusCodes.Status403Forbidden,
                _ => StatusCodes.Status500InternalServerError
            };

            var errors = new Dictionary<string, string[]>
            {
                [error.Code] = [error.Description]
            };

            var response = new ApiErrorResponse(
                Success: false,
                Message: error.Description,
                Errors: errors,
                TraceId: HttpContext.TraceIdentifier);

            return StatusCode(statusCode, response);
        }

        private ActionResult ValidationProblem(List<Error> errors)
        {
            var errorsDictionary = errors
                .GroupBy(error => error.Code)
                .ToDictionary(
                    group => group.Key,
                    group => group
                        .Select(error => error.Description)
                        .ToArray());

            var response = new ApiErrorResponse(
                Success: false,
                Message: "Validation failed",
                Errors: errorsDictionary,
                TraceId: HttpContext.TraceIdentifier);

            return BadRequest(response);
        }

    }
}
