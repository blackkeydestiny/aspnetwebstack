// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Diagnostics.Contracts;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Hosting;
using System.Web.Http.Properties;
using System.Web.Http.Routing;

namespace System.Web.Http.Dispatcher
{
    /*
     * 2个基本职能：路由和消息分发
     * 路由职能：它会调用当前路由表对请求消息实施路由解析进而生成用于封装路由数据的HttpRouteData
     * 消息分发职能：会将请求直接分发给在创建时指定的HttpMessageHandler来完成进—步处理
     * **/

    /// <summary>
    /// This class is the default endpoint message handler which examines the <see cref="IHttpRoute"/>
    /// of the matched route, and chooses which message handler to call. If <see cref="IHttpRoute.Handler"/>
    /// is <c>null</c>, then it delegates to <see cref="HttpControllerDispatcher"/>.
    /// </summary>
    public class HttpRoutingDispatcher : HttpMessageHandler
    {


        private readonly HttpConfiguration _configuration;
        private readonly HttpMessageInvoker _defaultInvoker;



        #region 构造方法
        //===========================================构造方法======================================================================
        /*
         * 注意：第二个参数为HttpControllerDispatcher，HttpControllerDispatcher具体完成目标contoller的激活和action方法的执行以及响应的生成
         * 
         * **/
        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoutingDispatcher"/> class,
        /// using the provided <see cref="HttpConfiguration"/> and <see cref="HttpControllerDispatcher"/>
        /// as the default handler.
        /// </summary>
        /// <param name="configuration">The server configuration.</param>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "HttpControllerDispatcher does not require disposal")]
        public HttpRoutingDispatcher(HttpConfiguration configuration)
            : this(configuration, new HttpControllerDispatcher(configuration))
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRoutingDispatcher"/> class,
        /// using the provided <see cref="HttpConfiguration"/> and <see cref="HttpMessageHandler"/>.
        /// </summary>
        /// <param name="configuration">The server configuration.</param>
        /// <param name="defaultHandler">The default handler to use when the <see cref="IHttpRoute"/> has no <see cref="IHttpRoute.Handler"/>.</param>
        public HttpRoutingDispatcher(HttpConfiguration configuration, HttpMessageHandler defaultHandler)
        {
            if (configuration == null)
            {
                throw Error.ArgumentNull("configuration");
            }
            if (defaultHandler == null)
            {
                throw Error.ArgumentNull("defaultHandler");
            }

            _configuration = configuration;
            _defaultInvoker = new HttpMessageInvoker(defaultHandler);
        }
        //===========================================构造方法======================================================================
        #endregion



        /*
         * 重写SendAsync方法
         * 
         * **/
        //===============================================SendAsync方法==================================================================
        /// <inheritdoc/>
        [SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Justification = "The Web API framework will dispose of the response after sending it")]
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // Lookup route data, or if not found as a request property then we look it up in the route table
            IHttpRouteData routeData = request.GetRouteData();
            if (routeData == null)
            {
                routeData = _configuration.Routes.GetRouteData(request);
                if (routeData != null)
                {
                    request.SetRouteData(routeData);
                }
            }

            if (routeData == null || (routeData.Route != null && routeData.Route.Handler is StopRoutingHandler))
            {
                request.Properties.Add(HttpPropertyKeys.NoRouteMatched, true);

                // 404 Not Found
                return Task.FromResult(request.CreateErrorResponse(
                    HttpStatusCode.NotFound,
                    Error.Format(SRResources.ResourceNotFound, request.RequestUri),
                    SRResources.NoRouteData));
            }

            routeData.RemoveOptionalRoutingParameters();

            // routeData.Route could be null if user adds a custom route that derives from System.Web.Routing.Route explicitly 
            // and add that to the RouteCollection in the web hosted case
            var invoker = (routeData.Route == null || routeData.Route.Handler == null) ?
                _defaultInvoker : new HttpMessageInvoker(routeData.Route.Handler, disposeHandler: false);
            return invoker.SendAsync(request, cancellationToken);
        }
        //===============================================SendAsync方法==================================================================
    }
}
