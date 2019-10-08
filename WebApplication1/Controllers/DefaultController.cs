using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace WebApplication1.Controllers
{
    public class DefaultController : ApiController
    {
        public IHttpActionResult GetHello()
        {
            return this.ToJson(new HelloModule
            {
                Now = DateTime.Now,
                Last = DateTime.Now.AddDays(1)
            });
        }
    }

    public class HelloModule
    {
        public DateTime Now { get; set; }
        public DateTime Last { get; set; }
    }
}
