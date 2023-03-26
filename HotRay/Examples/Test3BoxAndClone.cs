using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Nodes.Components.Filters;
using HotRay.Base.Nodes.Components.Processors;
using HotRay.Base.Nodes.Components.Utils;
using HotRay.Base.Nodes.Sources;
using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Examples
{
    internal static class Test3BoxAndClone
    {
        public static void Run()
        {
            using Space space = new Space()
            {
                TicksPerSecond = 5,
                PrintTickInfo = true,
            };
            space.LogEvent += s => Console.WriteLine(s);

            var source = space.CreateNode<PulseSource>();
            var spread = space.CreateNode<Spread<SignalRay>>();

            var box1 = ConstructBox(space);
            var box2 = new Box(box1);

            box1.SetExtraInfo("IntValue", 1);
            box2.SetExtraInfo("IntValue", 2);

            source.OutPorts[0].ConnectTo(spread.InPorts[0]);
            spread.OutPorts[0].ConnectTo(box1.InPorts[0]);
            spread.OutPorts[1].ConnectTo(box2.InPorts[0]);
            

            Console.WriteLine(space.LayoutToString());


            var task = space.StartAndRunAsync();
            task.Wait();
        }


        static Box ConstructBox(Space space)
        {
            var box = space.CreateNode<Box>();

            box.AddInPort<SignalRay>();

            var filter = box.CreateNode<SignalToInt>();
            var print = box.CreateNode<Print<BoolRay>>();

            box.RegisterSetExtraInfoCallback("IntValue", new Box.ExtraInfoData.CallbackInfo()
            {
                Receiver = filter,
                Callback = (r, o, n) =>
                {
                    if(n is int i)
                        (r as SignalToInt)!.EmitValue = i;
                }
            });

            box.InPortInnerReflections[0].ConnectTo(filter.InPorts[0]);
            filter.OutPorts[0].ConnectTo(print.InPorts[0]);

            return box;
        }
        
        /*
        public static void Run()
        {
            using Space space = new Space()
            {
                TicksPerSecond = 5,
                PrintTickInfo = true,
                MaxNodePerTick = -1,
            };

            var source = space.CreateNode<PulseSource>();
            var spread = space.CreateNode<Spread<SignalRay>>();
            var filter1 = space.CreateNode<SignalToInt>();
            var filter2 = space.CreateNode<SignalToInt>();
            var filter3 = space.CreateNode<SignalToInt>();
            var filter4 = space.CreateNode<SignalToInt>();
            var box1 = CreateBox(space);
            var box2 = box1.CloneNode();
            var print1 = space.CreateNode<Print<IntRay>>();
            var print2 = space.CreateNode<Print<IntRay>>();

            source.Interval = 0;
            source.Count = -1;
            spread.PortNum = 4;
            filter1.EmitValue = 1;
            filter2.EmitValue = 10;
            filter3.EmitValue = 100;
            filter4.EmitValue = 1000;

            source.OutPorts[0].ConnectTo(spread.InPorts[0]);

            spread.OutPorts[0].ConnectTo(filter1.InPorts[0]);
            spread.OutPorts[1].ConnectTo(filter2.InPorts[0]);
            spread.OutPorts[2].ConnectTo(filter3.InPorts[0]);
            spread.OutPorts[3].ConnectTo(filter4.InPorts[0]);

            filter1.OutPorts[0].ConnectTo(box1.InPorts[0]);
            filter2.OutPorts[0].ConnectTo(box1.InPorts[1]);
            filter3.OutPorts[0].ConnectTo(box2.InPorts[0]);
            filter4.OutPorts[0].ConnectTo(box2.InPorts[1]);

            box1.OutPorts[0].ConnectTo(print1.InPorts[0]);
            box2.OutPorts[0].ConnectTo(print2.InPorts[0]);


            Console.WriteLine(space.LayoutToString());

            space.LogEvent += s => Console.WriteLine(s);
            space.Init();

            var task = space.RunAsync();
            task.Wait();
        }


        static Box CreateBox(Space space)
        {
            var box1 = space.CreateNode<Box>();

            box1.AddInPort<IntRay>();
            box1.AddInPort<IntRay>();
            box1.AddOutPort<IntRay>();

            var adder1 = box1.CreateNode<ProcessorBase<AdderCore>>();
            var adder2 = box1.CreateNode<ProcessorBase<AdderCore>>();
            var spread = box1.CreateNode<Spread<IntRay>>();

            spread.PortNum = 2;

            box1.InPortInnerReflections[0].ConnectTo(adder1.InPorts[0]);
            box1.InPortInnerReflections[1].ConnectTo(spread.InPorts[0]);

            spread.OutPorts[0].ConnectTo(adder1.InPorts[1]);

            adder1.OutPorts[0].ConnectTo(adder2.InPorts[0]);
            spread.OutPorts[1].ConnectTo(adder2.InPorts[1]);

            adder2.OutPorts[0].ConnectTo(box1.OutPortInnerReflections[0]);

            return box1;
        }*/
    }
}
