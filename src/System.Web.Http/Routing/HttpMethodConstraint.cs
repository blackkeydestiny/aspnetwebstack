// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net.Http;

namespace System.Web.Http.Routing
{
    public class HttpMethodConstraint : IHttpRouteConstraint
    {
        public HttpMethodConstraint(params HttpMethod[] allowedMethods)
        {
            if (allowedMethods == null)
            {
                throw Error.ArgumentNull("allowedMethods");
            }

            AllowedMethods = new Collection<HttpMethod>(allowedMethods);
        }

        /*
         * 
         *  HttpMethod的集合,当Match方法被执行的时候,它会从代表被检验请求的HttpRequestMessage对象中获得采用的HTTP方法,并根据此列表做出是否满足约束的最终判断。
         *  
         *  用于验证数据类型的IntRouteConstraint、FloatRouteConstraint和BoolRouteConstraint,用于验证字符串长度的LenghRouteConstraint、MinlengthRouteConstraint和MaxLengtllRouteConstraint等 
         * **/
        public Collection<HttpMethod> AllowedMethods { get; private set; }

        protected virtual bool Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            if (request == null)
            {
                throw Error.ArgumentNull("request");
            }

            if (route == null)
            {
                throw Error.ArgumentNull("route");
            }

            if (parameterName == null)
            {
                throw Error.ArgumentNull("parameterName");
            }

            if (values == null)
            {
                throw Error.ArgumentNull("values");
            }

            switch (routeDirection)
            {
                case HttpRouteDirection.UriResolution:
                    return AllowedMethods.Contains(request.Method);

                case HttpRouteDirection.UriGeneration:
                    // We need to see if the user specified the HTTP method explicitly.  Consider these two routes:
                    //
                    // a) Route: template = "/{foo}", Constraints = { httpMethod = new HttpMethodConstraint("GET") }
                    // b) Route: template = "/{foo}", Constraints = { httpMethod = new HttpMethodConstraint("POST") }
                    //
                    // A user might know ahead of time that a URI he/she is generating might be used with a particular HTTP
                    // method.  If a URI will be used for an HTTP POST but we match on (a) while generating the URI, then
                    // the HTTP GET-specific route will be used for URI generation, which might have undesired behavior.
                    // To prevent this, a user might call RouteCollection.GetVirtualPath(..., { httpMethod = "POST" }) to
                    // signal that he is generating a URI that will be used for an HTTP POST, so he wants the URI
                    // generation to be performed by the (b) route instead of the (a) route, consistent with what would
                    // happen on incoming requests.
                    HttpMethod constraint;
                    if (!values.TryGetValue(parameterName, out constraint))
                    {
                        return true;
                    }

                    return AllowedMethods.Contains(constraint);

                default:
                    throw Error.InvalidEnumArgument(String.Empty, (int)routeDirection, typeof(HttpRouteDirection));
            }
        }

        bool IHttpRouteConstraint.Match(HttpRequestMessage request, IHttpRoute route, string parameterName, IDictionary<string, object> values, HttpRouteDirection routeDirection)
        {
            return Match(request, route, parameterName, values, routeDirection);
        }
    }
}
