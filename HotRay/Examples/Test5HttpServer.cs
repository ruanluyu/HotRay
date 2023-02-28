using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Nodes.Components.Filters;
using HotRay.Base.Nodes.Components.Logics;
using HotRay.Base.Nodes.Components.Utils;
using HotRay.Base.Nodes.Sources;
using HotRay.Base.Ray.Lite;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotRay.Base.Ray.Hot;

namespace HotRay.Examples
{
    internal static class Test5HttpServer
    {
        public static void Run()
        {
            Space space = new Space()
            {
                TicksPerSecond = 10,
                PrintTickInfo = false,
                MaxNodePerTick = -1
            };
            space.LogEvent += s => Console.WriteLine(s);

            var httpSource = space.CreateNode<HttpContextSource>();
            var print = space.CreateNode<Print<ObjectRay>>();

            // Command out this line to test https server. 
            // httpSource.URIPrefix = "https://+:44300/";

            httpSource.OutPorts[0].ConnectTo(print.InPorts[0]);

            var task = space.StartAndRunAsync();
            task.Wait();
            if (task.IsFaulted)
                Console.WriteLine(task.Exception?.InnerException);
        }
    }
}
