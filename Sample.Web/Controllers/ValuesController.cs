using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Sample.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ValuesController : ControllerBase
    {
        // GET api/values
        [HttpGet]
        public async Task<IEnumerable<string>> Get([FromServices] ITestProxy testProxy)
        {
            testProxy.Test();
            var result = testProxy.TestInt();
            await testProxy.TestAsync();
            var resuktAsync = await testProxy.ResultAsync();

            if (result > resuktAsync)
            {
                Console.WriteLine($"hello");
            }

            return new string[] { "value1", "value2", $"{result}", $"{resuktAsync}" };
        }
    }
}
