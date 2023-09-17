using RecSharp.Core.Hosting.Http;
using RecSharp.Core.Internal.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecSharp.Example.Servers
{
    internal class ApiServer : HttpServiceProvider
    {
        [HttpEndpoint("/test/{id}/nigger", HttpMethods.GET)]
        public string Root()
        {
            return $"Hello, World! ~ {GetDynamicField("id")}";
        }
    }
}