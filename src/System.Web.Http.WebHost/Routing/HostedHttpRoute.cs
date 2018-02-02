// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Net.Http;
using System.Web.Http.Routing;
using System.Web.Routing;

namespace System.Web.Http.WebHost.Routing
{
    /*
     * HttpRoute一样，参照HttpRoute学习
     * 我们可以将HostedHttpRoute对象看成对一个Route对象的封装 
     * 
     * **/
    internal class HostedHttpRoute : IHttpRoute
    {



        public HostedHttpRoute(string uriTemplate, IDictionary<string, object> defaults, IDictionary<string, object> constraints, IDictionary<string, object> dataTokens, HttpMessageHandler handler)
        {
            RouteValueDictionary routeDefaults = defaults != null ? new RouteValueDictionary(defaults) : null;
            RouteValueDictionary routeConstraints = constraints != null ? new RouteValueDictionary(constraints) : null;
            RouteValueDictionary routeDataTokens = dataTokens != null ? new RouteValueDictionary(dataTokens) : null;

            /*
             * 实现在HostedHttpRoute之中的核心路由功能基本上是通过这个Route对象完成的,
             * 所以我们才说ASP.NET Web API的路由在Web Host寄宿模式下还是利用ASP.NET自身的路由系统实现的
             * 
             * **/
            OriginalRoute = new HttpWebRoute(uriTemplate, routeDefaults, routeConstraints, routeDataTokens, HttpControllerRouteHandler.Instance, this);


            Handler = handler;
        }


        // ===========================================属性=========================================================================
        public string RouteTemplate
        {
            get { return OriginalRoute.Url; }
        }

        public IDictionary<string, object> Defaults
        {
            get { return OriginalRoute.Defaults; }
        }

        public IDictionary<string, object> Constraints
        {
            get { return OriginalRoute.Constraints; }
        }

        public IDictionary<string, object> DataTokens
        {
            get { return OriginalRoute.DataTokens; }
        }
        // ===========================================属性=========================================================================


        public HttpMessageHandler Handler { get; private set; }


        /*
         * 实现在HostedHttpRoute之中的核心路由功能基本上是通过这个Route对象完成的,
         * 所以我们才说ASP.NET Web API的路由在Web Host寄宿模式下还是利用ASP.NET自身的路由系统实现的
         * 
         * **/  
        internal Route OriginalRoute { get; private set; }






        // ===========================================GetRouteData=========================================================================
        public IHttpRouteData GetRouteData(string rootVirtualPath, HttpRequestMessage request)
        {
            if (rootVirtualPath == null)
            {
                throw Error.ArgumentNull("rootVirtualPath");
            }

            if (request == null)
            {
                throw Error.ArgumentNull("request");
            }

            HttpContextBase httpContextBase = request.GetHttpContext();
            if (httpContextBase == null)
            {
                httpContextBase = new HttpRequestMessageContextWrapper(rootVirtualPath, request);
            }

            RouteData routeData = OriginalRoute.GetRouteData(httpContextBase);
            if (routeData != null)
            {
                return new HostedHttpRouteData(routeData);
            }

            return null;
        }
        // ===========================================GetRouteData=========================================================================





        // ===========================================GetVirtualPath=========================================================================
        public IHttpVirtualPathData GetVirtualPath(HttpRequestMessage request, IDictionary<string, object> values)
        {
            if (request == null)
            {
                throw Error.ArgumentNull("request");
            }

            HttpContextBase httpContextBase = request.GetHttpContext();
            if (httpContextBase != null)
            {
                HostedHttpRouteData routeData = request.GetRouteData() as HostedHttpRouteData;
                if (routeData != null)
                {
                    RequestContext requestContext = new RequestContext(httpContextBase, routeData.OriginalRouteData);
                    VirtualPathData virtualPathData = OriginalRoute.GetVirtualPath(requestContext, new RouteValueDictionary(values));
                    if (virtualPathData != null)
                    {
                        return new HostedHttpVirtualPathData(virtualPathData, routeData.Route);
                    }
                }
            }

            return null;
        }
        // ===========================================GetVirtualPath=========================================================================

    }
}
