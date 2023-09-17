using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecSharp.Core.Internal.Http
{
    public class HttpAuthorizationAttribute : Attribute
    {
        public HttpAuthorizationAttribute(HttpAuthorization authorizationType)
        {
            AuthorizationType = authorizationType;
        }

        public HttpAuthorization AuthorizationType { get; set; }
    }
}
