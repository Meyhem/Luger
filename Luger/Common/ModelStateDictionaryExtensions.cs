using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Luger.Common
{
    public static class ModelStateDictionaryExtensions
    {
        public static ObjectResult ToProblemResult(this ModelStateDictionary self)
        {
            var problem = new ProblemDetails
            {
                Status = 400,
                Type = "ValidationError",
                Title = "Failed to validate input data",
                Detail = self.Values.FirstOrDefault()?.Errors.FirstOrDefault()?.ErrorMessage
            };
            
            return new ObjectResult(problem)
            {
                StatusCode = 400
            };
        }
    }
}