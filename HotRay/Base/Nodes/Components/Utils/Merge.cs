using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class Merge<rayT>:ComponentBase where rayT:RayBase
    {
        protected Port<rayT>[] inPorts = new Port<rayT>[] { new Port<rayT>() };
        protected readonly Port<rayT> outPort0 = new Port<rayT>();

        public Merge() : base()
        {
            PortNum = 2;
        }

        public Merge(int count) : base()
        {
            PortNum = count;
        }


        public Merge(Merge<rayT> other) : base(other)
        {
            PortNum = other.PortNum;
        }

        public int PortNum
        {
            get => inPorts.Length;
            set
            {
                if (value < 0) PortNum = 0;

                if (inPorts.Length == value) return;

                inPorts = new Port<rayT>[value];
                for (int i = 0; i < value; i++)
                {
                    inPorts[i] = new Port<rayT>();
                }
            }
        }
        public override IReadOnlyList<IPort> OutPorts => new IPort[] { outPort0 };

        public override IReadOnlyList<IPort> InPorts => inPorts;

        public override INode CloneNode()
        {
            return new Merge<rayT>(this);
        }

        public override Status OnPortUpdate(IPort inport)
        {
            if (PortNum == 0) return Status.Shutdown;
            outPort0.Ray = inport.Ray;
            return Status.ShutdownAndEmit;
        }
    }
}
