// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

using System.Collections.Generic;

namespace System.Web.Http.Routing
{
    /*
     * 
     * 用于封装路由数据的类，实现了IRouteData接口以及2个属性，并在构造方法中初始化。
     * 
     * 
     * **/

    public class HttpRouteData : IHttpRouteData
    {
        private IHttpRoute _route;
        private IDictionary<string, object> _values;

        /*
         * 
         * 1、如杲调用这一个构迨函数(只包含—个唯—的参数route),其values属性会初始化成一个不包含任何元素的空HttpRouteValueDictionary对象 。
         * **/

        public HttpRouteData(IHttpRoute route)
            : this(route, new HttpRouteValueDictionary())
        {
        }

        /*
         * 2、
         * **/
        public HttpRouteData(IHttpRoute route, HttpRouteValueDictionary values)
        {
            if (route == null)
            {
                throw Error.ArgumentNull("route");
            }

            if (values == null)
            {
                throw Error.ArgumentNull("values");
            }

            _route = route;
            _values = values;
        }

        /*
         * HttpRoute对象
         * **/
        public IHttpRoute Route
        {
            get { return _route; }
        }

        /*
         * 解析出的路由变量
         * **/
        public IDictionary<string, object> Values
        {
            get { return _values; }
        }
    }
}
