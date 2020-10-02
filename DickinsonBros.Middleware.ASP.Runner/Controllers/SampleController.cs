using System.Threading.Tasks;
using DickinsonBros.Middleware.ASP.Runner.Models;
using Microsoft.AspNetCore.Mvc;

namespace DickinsonBros.Middleware.ASP.Runner.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SampleController : ControllerBase
    {
        [HttpPost]
        public async Task<SampleResponse> Post(SampleRequest sampleRequest)
        {
            await Task.CompletedTask.ConfigureAwait(false);

            return new SampleResponse
            {
                MassiveString = "SampleString",
                UserId = "User100"
            };
        }
    }
}
