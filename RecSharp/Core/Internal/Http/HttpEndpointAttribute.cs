using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecSharp.Core.Internal.Http
{
    public class HttpEndpointAttribute : Attribute
    {
        public HttpEndpointAttribute(string url, HttpMethods method = HttpMethods.GET)
        {
            Url = url;
            HttpMethod = method;
        }

        public string Url { get; set; }
        public HttpMethods HttpMethod { get; set; }
    }
}
