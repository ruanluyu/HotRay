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


        protected readonly Port<rayT> inPort0 = new Port<rayT>();
        protected Port<rayT>[] outPorts = new Port<rayT>[] { new Port<rayT>() };

        public Spread():base()
        {
        }

        public Spread(int count):base()
        {
            PortNum = count;
        }


        public Spread(Spread<rayT> other) :base(other)
        {
            PortNum = other.PortNum;
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

        public override IReadOnlyList<IPort> InPorts => new IPort[] { inPort0 };

        public override IReadOnlyList<IPort> OutPorts => outPorts;

        public override INode CloneNode()
        {
            return new Spread<rayT>(this);
        }

        public override Status OnPortUpdate(IPort inport)
        {
            if (PortNum == 0) return Status.Shutdown;

            var refRay = inPort0.Ray;
            // inPort0.Ray = null;

            outPorts[0].Ray = refRay;
            for (int i = 1; i < outPorts.Length; i++)
                outPorts[i].Ray = refRay?.RayClone();

            return Status.ShutdownAndEmit;
        }

    }
}
