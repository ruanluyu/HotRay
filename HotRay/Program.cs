﻿using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Nodes.Components.Filters;
using HotRay.Base.Nodes.Components.Processors;
using HotRay.Base.Nodes.Components.Utils;
using HotRay.Base.Nodes.Sources;
using HotRay.Base.Port;
using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;

namespace HotRay
{
    class AdderCore : ICore
    {
        [InPort(0)]
        IntRay? a { get; set; }
        [InPort(1)]
        IntRay? b { get; set; }

        [OutPort(0)]
        IntRay? o { get; set; }

        public ICore CloneCore() => new AdderCore();

        public void Process()
        {
            o = new IntRay() { Data = a.Data + b.Data };
        }
    }


    public class Program
    {
        static void Main(string[] args)
        {
            Test2();
        }

        static void Test1()
        {
            Space space = new Space() { MsPerTick = 300, PrintTick = true };

            var pulse = space.CreateNode<PulseSource>();
            pulse.Interval = 3;
            pulse.Count = 10;

            var delayer = space.CreateNode<Delayer<SignalRay>>();
            pulse.OutPorts[0].ConnectTo(delayer.InPorts[0]);

            var intFilter = space.CreateNode<SignalToInt>();
            delayer.OutPorts[0].ConnectTo(intFilter.InPorts[0]);

            var passby = space.CreateNode<PassBy<IntRay>>();
            intFilter.OutPorts[0].ConnectTo(passby.InPorts[0]);

            var intFilter2 = space.CreateNode<SignalToInt>();
            intFilter2.EmitValue = 5;
            passby.OutPorts[1].ConnectTo(intFilter2.InPorts[0]);
            
            var adder = space.CreateNode<ProcessorBase<AdderCore>>();
            passby.OutPorts[0].ConnectTo(adder.InPorts[0]);
            intFilter2.OutPorts[0].ConnectTo(adder.InPorts[1]);

            var print = space.CreateNode<Print<IntRay>>();
            print.Newline = true;
            adder.OutPorts[0].ConnectTo(print.InPorts[0]);

            space.Init();
            var task = space.Run();
            task.Wait();
        }

        static void Test2()
        {
            Space space = new Space() { MsPerTick = 300, PrintTick = true };

            var pulse = space.CreateNode<PulseSource>();
            // pulse.Interval = 3;
            pulse.Count = 1;

            var orgate = space.CreateNode<OrGate>();
            pulse.OutPorts[0].ConnectTo(orgate.InPorts[0]);

            var delayer = space.CreateNode<Delayer<SignalRay>>();
            delayer.Delay = 3;
            orgate.OutPorts[0].ConnectTo(delayer.InPorts[0]);
            delayer.OutPorts[0].ConnectTo(orgate.InPorts[1]);

            var passby = space.CreateNode<PassBy<SignalRay>>();
            passby.InsertAfter(orgate.OutPorts[0]);

            var intfilter = space.CreateNode<SignalToInt>();
            passby.OutPorts[1].ConnectTo(intfilter.InPorts[0]);

            var print = space.CreateNode<Print<IntRay>>();
            print.Newline = true;
            intfilter.OutPorts[0].ConnectTo(print.InPorts[0]);

            space.Init();
            var task = space.Run();
            task.Wait();
        }
    }
}