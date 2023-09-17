using RecSharp.Core.Internal.Http;
using RecSharp.Core.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace RecSharp.Core.Hosting.Http
{
    public class HttpServiceProvider : IHttpServiceProvider
    {
        /// <summary>
        /// Attempts to start the HTTP service.
        /// </summary>
        /// <param name="port">The port that the service will listen on. (If it's 0, the app will requre admin privlages) (IT PROBABLY WILL CAUSE ISSUES)</param>
        public void StartService(int port = 0)
        {
            this.FindAllHttpEndpoints();
            this.RequestServiceStart(port);
        }

        private void RequestServiceStart(int port = 0)
        {
            string serviceUrl = "http://";
            if(port > 0)
            {
                serviceUrl += "localhost:" + port + "/";
            }
            else
            {
                serviceUrl += "+/";
            }

            this.HttpListener.Prefixes.Add(serviceUrl);
            this.ServiceUrl = serviceUrl;
            this.ServicePort = port;
            Constants.Logger.Info($"[{GetType().Name}] Attempting to start service...");
            this.StartService();
        }

        private void StartService()
        {
            this.HttpListener.Start();
            this.OnServiceRequest += this.HandleIncomingRequest;
            new Thread(new ThreadStart(() =>
            {
                this.ServiceRequestLoop();
            })).Start();
            Constants.Logger.Info($"[{GetType().Name}] Service Successfully started. ({this.ServiceUrl})");
        }

        private void ServiceRequestLoop()
        {
            while (true)
            {
                this.Context = this.HttpListener.GetContext();
                this.Request = this.Context.Request;
                this.Response = this.Context.Response;
                this.OnServiceRequest();
            }
        }

        public virtual void LogHttpRequest()
        {

        }

        private void HandleIncomingRequest()
        {
            HttpEndpoint currentEndpoint = this.GetEndpointFromUrl(this.QuerylessUrl);
            if(currentEndpoint == null)
            {
                this.WriteResponseWithStatus("The requested enpoint was not found.", 404);
                return;
            }
            else if(currentEndpoint.HttpMethod != Enum.Parse<HttpMethods>(this.Request.HttpMethod))
            {
                this.WriteResponseWithStatus($"The HTTP method \"{this.Request.HttpMethod}\" is not supported.", 405);
                return;
            }

            MethodInfo methodBase = GetType().GetMethod(currentEndpoint.MethodName);

            this.WriteResponse(methodBase.Invoke(this, null) as string);
        }

        private HttpEndpoint GetEndpointFromUrl(string url)
        {
            HttpEndpoint res = null;

            foreach(HttpEndpoint endpoint in this.CurrentEnpointList)
            {
                if(HttpEndpoint.Matches(url, endpoint))
                {
                    this.Endpoint = endpoint;
                    res = endpoint;
                }
            }

            return res;
        }

        private void WriteResponse(string msg)
        {
            this.HttpLogger.RequestLog(new()
            {
                Url = this.QuerylessUrl,
                HttpMethod = Enum.Parse<HttpMethods>(this.Request.HttpMethod),
                BodyData = this.BodyData,
                ContentType = this.Request.ContentType,
                Headers = this.Request.Headers,
                QueryStrings = this.Request.QueryString,
                Response = msg,
                Status = this.Response.StatusCode.ToString(),
                Service = GetType().Name
            });

            byte[] b = Encoding.UTF8.GetBytes(msg);
            this.Response.ContentLength64 = b.Length;
            this.Response.OutputStream.Write(b, 0, b.Length);
            this.Response.OutputStream.Flush();
        }
        
        private void WriteResponse(byte[] data)
        {
            this.HttpLogger.RequestLog(new()
            {
                Url = this.QuerylessUrl,
                HttpMethod = Enum.Parse<HttpMethods>(this.Request.HttpMethod),
                BodyData = this.BodyData,
                ContentType = this.Request.ContentType,
                Headers = this.Request.Headers,
                QueryStrings = this.Request.QueryString,
                Response = "nigga",
                Status = this.Response.StatusCode.ToString(),
                Service = GetType().Name
            });

            this.Response.ContentLength64 = data.Length;
            this.Response.OutputStream.Write(data, 0, data.Length);
            this.Response.OutputStream.Flush();
        }

        private void WriteResponseWithStatus(string msg, int statusCode)
        {
            this.Response.StatusCode = statusCode;
            this.WriteResponse(msg);
        }
        
        private void WriteResponseWithStatus(byte[] data, int statusCode)
        {
            this.Response.StatusCode = statusCode;
            this.WriteResponse(data);
        }

        private void FindAllHttpEndpoints()
        {
            foreach(MethodInfo mInfo in GetType().GetMethods())
            {
                HttpEndpointAttribute httpEndpointAttr = mInfo.GetCustomAttribute<HttpEndpointAttribute>();
                HttpAuthorizationAttribute httpAuthorizationAttr = mInfo.GetCustomAttribute<HttpAuthorizationAttribute>();
                bool endpointHasAuth = httpEndpointAttr != null;

                if(httpEndpointAttr != null)
                {
                    HttpEndpoint httpEndpoint = new HttpEndpoint(httpEndpointAttr.Url, httpEndpointAttr.HttpMethod, mInfo.Name, endpointHasAuth, httpAuthorizationAttr);
                    if(!this.CurrentEnpointList.Contains(httpEndpoint))
                    {
                        Constants.Logger.Trace($"[{GetType().Name}] Found new HTTP endpoint! (Url: \"{httpEndpoint.Url}\", Attached method name: \"{httpEndpoint.MethodName}\", Return type: {mInfo.ReturnType.Name})");
                        this.CurrentEnpointList.Add(httpEndpoint);
                    }
                }
            }
        }

        public string GetDynamicField(string fieldName)
        {
            if(!this.Endpoint.IsDynamicUrl)
            {
                return string.Empty;
            }
            foreach(var dynamicPart in Endpoint.DynamicParts)
            {
                if (dynamicPart.Value == fieldName)
                {
                    return QuerylessUrl.Split("/")[dynamicPart.Index];
                }
            }
            return string.Empty;
        }

        // HTTP Listener Stuff
        /// <summary>
        /// The current <see cref="System.Net.HttpListener"/> we are using.
        /// </summary>
        private HttpListener HttpListener { get; set; } = new HttpListener();

        /// <summary>
        /// The current context of the service.
        /// </summary>
        private HttpListenerContext Context { get; set; }
        
        /// <summary>
        /// The current request of the service.
        /// </summary>
        public HttpListenerRequest Request { get; set; }

        /// <summary>
        /// The current response of the service.
        /// </summary>
        private HttpListenerResponse Response { get; set; }

        // Request Data
        
        /// <summary>
        /// The current request url. (Queryless)
        /// </summary>
        public string QuerylessUrl
        {
            get
            {
                if (this.Request.RawUrl.Contains("?"))
                    return this.Request.RawUrl.Split("?")[0];
                return this.Request.RawUrl;
            }
        }

        /// <summary>
        /// The current request url. (With Query Strings)
        /// </summary>
        public string RawUrl => this.Request.RawUrl;

        public string BodyData
        {
            get
            {
                return new StreamReader(this.Request.InputStream, this.Request.ContentEncoding).ReadToEnd();
            }
        }

        public HttpEndpoint Endpoint { get; private set; }

        public byte[] RawBodyData
        {
            get
            {
                MemoryStream stream = new MemoryStream();
                this.Request.InputStream.CopyTo(stream);
                return stream.ToArray();
            }
        }

        // Service Data
        /// <summary>
        /// The current url that is being used for the HTTP service.
        /// </summary>
        public string ServiceUrl { get; set; } = "";

        /// <summary>
        /// The current port that is being used for the HTTP service.
        /// </summary>
        public int ServicePort { get; set; } = 0;

        /// <summary>
        /// All the HTTP endpoints in the class.
        /// </summary>
        private List<HttpEndpoint> CurrentEnpointList { get; set; } = new List<HttpEndpoint>();

        // Response Data

        public string ResponseString { get; private set; }

        // Events

        /// <summary>
        /// This fires when a new HTTP request comes through.
        /// </summary>
        public event Action OnServiceRequest;



        private HttpLogger HttpLogger { get; set; } = new HttpLogger();
    }
}
