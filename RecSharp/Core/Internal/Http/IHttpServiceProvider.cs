using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RecSharp.Core.Internal.Http
{
    internal interface IHttpServiceProvider
    {
        void StartService(int port);
    }
}
