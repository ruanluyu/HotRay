using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Nodes.Components.Filters;
using HotRay.Base.Nodes.Components.Logics;
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
            Test3();
        }

        static void Test1()
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

        static void Test2()
        {
            Space space = new Space() 
            {
                TicksPerSecond = 10, 
                PrintTickInfo = true, 
                MaxNodePerTick = -1 
            };
            

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



            space.LogEvent += s => Console.WriteLine(s);
            space.Init();
            var task = space.RunAsync();
            
            task.Wait();

            if(task.IsFaulted)
            {
                Console.WriteLine(task.Exception?.InnerException);
            }
        }

        static void Test3() // Dead lock
        {
            using Space space = new Space()
            {
                TicksPerSecond = 5,
                PrintTickInfo = true,
                MaxNodePerTick = -1,
                MaxTick = 10
            };
            var pulse = space.CreateNode<PulseSource>();
            var orgate = space.CreateNode<Merge<SignalRay>>();
            var delayer = space.CreateNode<Delayer<SignalRay>>();

            pulse.Count = 1;
            orgate.PortNum = 2;


            pulse.OutPorts[0].ConnectTo(orgate.InPorts[0]);
            orgate.OutPorts[0].ConnectTo(delayer.InPorts[0]);
            delayer.OutPorts[0].ConnectTo(orgate.InPorts[1]); // Dead lock


            space.LogEvent += s => Console.WriteLine(s);
            space.Init();

            var task = space.RunAsync();
            task.Wait();
        }
    }
}