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

namespace HotRay.Examples
{
    internal static class Test2LoopSignals
    {
        public static void Run()
        {
            Space space = new Space()
            {
                TicksPerSecond = 10,
                PrintTickInfo = true,
                MaxNodePerTick = -1
            };
            space.LogEvent += s => Console.WriteLine(s);

            var pulse = space.CreateNode<PulseSource>();
            var orgate = space.CreateNode<OrGate>();
            var delayer = space.CreateNode<Delayer<SignalRay>>();
            var passby = space.CreateNode<PassBy<SignalRay>>();
            var intfilter = space.CreateNode<SignalToInt>();
            var print = space.CreateNode<Print<IntRay>>();

            pulse.Interval = 2;
            pulse.Count = 2;
            delayer.Delay = 5;
            print.Newline = true;


            pulse.OutPorts[0].ConnectTo(orgate.InPorts[0]);
            orgate.OutPorts[0].ConnectTo(delayer.InPorts[0]);
            delayer.OutPorts[0].ConnectTo(orgate.InPorts[1]);
            passby.InsertAfter(orgate.OutPorts[0]);
            passby.OutPorts[1].ConnectTo(intfilter.InPorts[0]);
            intfilter.OutPorts[0].ConnectTo(print.InPorts[0]);



            
            var task = space.RunAsync();

            task.Wait();

            if (task.IsFaulted)
            {
                Console.WriteLine(task.Exception?.InnerException);
            }
        }
    }
}
