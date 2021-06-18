using Luger.Api.Endpoints.Models;
using Microsoft.AspNetCore.Mvc;

namespace Luger.Api.Endpoints
{
    public class LugerControllerBase: Controller
    {
        public BadRequestObjectResult BadRequestModelState()
        {
            return BadRequest(ApiResponse.FromError(LugerError.FromModelState(ModelState)));
        }
    }
}
