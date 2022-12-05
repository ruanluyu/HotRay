using HotRay.Base.Nodes.Components.Processors;
using HotRay.Base.Port;
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
        FloatRay? o { get; set; }

        public object Clone() => new AdderCore();

        public void CopyFrom(ICore other)
        {
            throw new NotImplementedException();
        }

        public void Process()
        {
            o = new FloatRay() { Data = a.Data + b.Data };
        }
    }


    public class Program
    {
        static void Main(string[] args)
        {
            var i1 = new IntRay() { Data = 5 };
            var i2 = new IntRay() { Data = -9 };
            var adder = new ProcessorBase<AdderCore>();
            adder.InputPorts[0].Ray = i1;
            adder.InputPorts[1].Ray = i2;
            var r = adder.GetRoutine();
            while(r.MoveNext())
            {
                if (r.Current == Base.Nodes.Status.Shutdown) throw new Exception();
            }
            Console.WriteLine((adder.OutputPorts[0].Ray as FloatRay)!.Data);
            Console.WriteLine(adder.InputPorts[0].ConnectableTo(adder.InputPorts[1]));
            Console.WriteLine(adder.OutputPorts[0].ConnectableTo(adder.InputPorts[1]));
            Console.WriteLine(adder.InputPorts[1].ConnectableTo(adder.OutputPorts[0]));
        }
    }
}