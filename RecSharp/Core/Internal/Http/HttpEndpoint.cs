using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecSharp.Core.Internal.Http
{
    public class HttpEndpoint
    {
        public HttpEndpoint(string url, HttpMethods httpMethod, string methodName, bool requiresAuthorization, HttpAuthorizationAttribute? authorization)
        {
            Url = url;
            HttpMethod = httpMethod;
            MethodName = methodName;
            RequiresAuthorization = requiresAuthorization;
            Authorization = authorization;
        }

        public bool IsDynamicUrl
        {
            get
            {
                Regex dynamicUrl = new Regex("{.*}");
                return dynamicUrl.IsMatch(Url);
            }
        }

        public List<DynamicUrlPart> DynamicParts
        {
            get
            {
                if(!IsDynamicUrl)
                {
                    return new List<DynamicUrlPart>();
                }
                var retVal = new List<DynamicUrlPart>();
                var urlParts = Url.Split("/");
                int index = 0;
                foreach(var part in urlParts)
                {
                    if(part.StartsWith("{") && part.EndsWith("}"))
                    {
                        retVal.Add(new DynamicUrlPart(part.Replace("{", "").Replace("}", ""), index));
                    }
                    index++;
                }
                return retVal;
            }
        }

        public static bool Matches(string url, HttpEndpoint endpoint)
        {
            if(!endpoint.IsDynamicUrl)
            {
                return endpoint.Url == url;
            }
            var urlParts = url.Split('/');
            var endpointUrlParts = endpoint.Url.Split("/");
            if(urlParts.Length != endpointUrlParts.Length)
            {
                return false;
            }
            return true;
        }

        public string Url { get; set; }
        public HttpMethods HttpMethod { get; set; }
        public string MethodName { get; set; }
        public bool RequiresAuthorization { get; set; }
        public HttpAuthorizationAttribute? Authorization { get; set; }

        public class DynamicUrlPart
        {
            public DynamicUrlPart(string value, int index)
            {
                Value = value;
                Index = index;
            }

            public string Value { get; set; }
            public int Index { get; set; }
        }
    }
}
