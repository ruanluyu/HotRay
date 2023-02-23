using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Nodes.Components.Filters;
using HotRay.Base.Nodes.Components.Processors;
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
    internal static class Test1AddTwoNumbers
    {
        public static void Run()
        {
            Space space = new Space() { TicksPerSecond = 20, PrintTickInfo = false };

            var pulse = space.CreateNode<PulseSource>();
            var spread = space.CreateNode<Spread<SignalRay>>();
            var intFilter = space.CreateNode<SignalToInt>();
            var intFilter2 = space.CreateNode<SignalToInt>();
            var adder = space.CreateNode<ProcessorBase<AdderCore>>();
            var print = space.CreateNode<Print<IntRay>>();



            pulse.Interval = 3;
            pulse.Count = 10;
            print.Newline = true;

            intFilter.EmitValue = 156;
            intFilter2.EmitValue = 687;

            pulse.OutPorts[0].ConnectTo(spread.InPorts[0]);

            spread.OutPorts[0].ConnectTo(intFilter.InPorts[0]);
            spread.OutPorts[1].ConnectTo(intFilter2.InPorts[0]);

            intFilter.OutPorts[0].ConnectTo(adder.InPorts[0]);
            intFilter2.OutPorts[0].ConnectTo(adder.InPorts[1]);

            adder.OutPorts[0].ConnectTo(print.InPorts[0]);



            space.LogEvent += s => Console.WriteLine(s);
            space.Init();
            var task = space.RunAsync();
            task.Wait();
        }
    }
}
