using System.Linq;
using Luger.Common;
using Luger.Features.Configuration;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Luger.Endpoints
{
    public class LugerControllerBase: Controller
    {
        // private readonly IOptions<LugerOptions> lugerOptions;

        public LugerControllerBase()
        {
           
        }
        
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var lugerOptions = HttpContext.RequestServices.GetRequiredService<IOptions<LugerOptions>>();
            
            base.OnActionExecuting(context);
            if (!context.ModelState.IsValid)
            {
                context.Result = ModelState.ToProblemResult();
            }

            var candidateRouteData = RouteData.Values.Where(v => v.Key == "bucket").ToArray();
            if (candidateRouteData.Any())
            {
                var bucketRouteParam = candidateRouteData.First();
                var bucketParamName = Normalization.NormalizeBucketName(bucketRouteParam.Value as string ?? string.Empty);
                if (lugerOptions.Value.Buckets.All(opt => opt.Id != bucketParamName))
                {
                    context.Result = Problem(detail: "No such bucket", statusCode: 404);
                }
            }
        }
    }
}
