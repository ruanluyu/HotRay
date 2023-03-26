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
using HotRay.Base.Nodes.Components.SAs;

namespace HotRay.Examples
{
    public static class Test5SequencerAndAccumulater
    {
        public static void Run()
        {
            Space space = new Space() 
            { 
                TicksPerSecond = 0, 
                PrintTickInfo = false 
            };
            space.LogEvent += s => Console.WriteLine(s);

            var pulse = space.CreateNode<PulseSource>();
            var stringFilter = space.CreateNode<SignalToString>();

            var stringIntSeq = space.CreateNode<StringIntSequencer>();
            var spread = space.CreateNode<Spread<IntRay>>();
            var print = space.CreateNode<Print<IntRay>>();
            var intStringAcc = space.CreateNode<IntStringAccumulater>();
            var print2 = space.CreateNode<Print<StringRay>>();


            // Comment out these 2 lines to turn off cross-tick-async feature. 
            stringIntSeq.EnableCrossTickAsync = true;
            intStringAcc.EnableCrossTickAsync = true;

            pulse.Interval = 3;
            pulse.Count = 10;
            stringFilter.EmitValue = "Test, 测试~ \u3215";

            print.Newline = true;
            print.Format = "X2";
            print.Template = "Print1: 0x{0}";

            print2.Newline = true;
            print2.Template = "Print2: {0}";

            spread.PortNum = 2;

            pulse.OutPorts[0].ConnectTo(stringFilter.InPorts[0]);
            stringFilter.OutPorts[0].ConnectTo(stringIntSeq.InPorts[0]);
            stringIntSeq.OutPorts[0].ConnectTo(spread.InPorts[0]);
            spread.OutPorts[0].ConnectTo(print.InPorts[0]);
            spread.OutPorts[1].ConnectTo(intStringAcc.InPorts[0]);
            intStringAcc.OutPorts[0].ConnectTo(print2.InPorts[0]);

            var task = space.StartAndRunAsync();
            task.Wait();
        }
    }
}
