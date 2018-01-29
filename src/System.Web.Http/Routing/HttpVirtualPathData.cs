// Copyright (c) Microsoft Open Technologies, Inc. All rights reserved. See License.txt in the project root for license information.

namespace System.Web.Http.Routing
{
    public class HttpVirtualPathData : IHttpVirtualPathData
    {
        private string _virtualPath;

        /*
         * 
         * **/
        public HttpVirtualPathData(IHttpRoute route, string virtualPath)
        {
            if (route == null)
            {
                throw Error.ArgumentNull("route");
            }

            if (virtualPath == null)
            {
                throw Error.ArgumentNull("virtualPath");
            }

            Route = route;
            VirtualPath = virtualPath;
        }

        public IHttpRoute Route { get; private set; }

        /*
         * 根据路由模板和指定路由变量生成一个完整的URL
         * **/
        public string VirtualPath
        {
            get { return _virtualPath; }
            set
            {
                if (value == null)
                {
                    throw Error.PropertyNull();
                }
                _virtualPath = value;
            }
        }
    }
}
