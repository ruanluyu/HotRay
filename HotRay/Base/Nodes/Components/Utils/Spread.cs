using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class Spread<rayT>: ComponentBase
        where rayT:RayBase
    {


        protected readonly Port<rayT> inPort0;
        protected Port<rayT>[] outPorts = Array.Empty<Port<rayT>>();

        public Spread():this(2)
        {
        }

        public Spread(int count):base()
        {
            PortNum = count;
            inPort0 = CreatePort<rayT>();
        }


        public Spread(Spread<rayT> other) :base(other)
        {
            PortNum = other.PortNum;
            inPort0 = CreatePort<rayT>();
        }

        public int PortNum
        {
            get => outPorts.Length;
            set
            {
                if (value < 0) PortNum = 0;

                if (outPorts.Length == value) return;

                outPorts = new Port<rayT>[value];
                for (int i = 0; i < value; i++)
                {
                    outPorts[i] = new Port<rayT>();
                }
            }
        }

        public override IReadOnlyList<PortBase> InPorts => new PortBase[] { inPort0 };

        public override IReadOnlyList<PortBase> OutPorts => outPorts;

        public override NodeBase CloneNode()
        {
            return new Spread<rayT>(this);
        }

        public override Status OnActivated()
        {
            if (PortNum == 0) return Status.Shutdown;


            var refRay = inPort0.Ray;
            // inPort0.Ray = null;
            if (refRay == outPorts[0].Ray) return Status.Shutdown;

            outPorts[0].Ray = refRay;
            for (int i = 1; i < outPorts.Length; i++)
                outPorts[i].Ray = refRay?.RayClone();

            return Status.ShutdownAndEmit;
        }

    }
}
