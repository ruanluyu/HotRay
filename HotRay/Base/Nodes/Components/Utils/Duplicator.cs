using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class Duplicator<rayT>: ComponentBase
        where rayT:RayBase
    {


        protected readonly Port<rayT> inPort0 = new Port<rayT>();
        protected Port<rayT>[] outPorts = new Port<rayT>[] { new Port<rayT>() };

        public Duplicator():base()
        {
        }

        public Duplicator(int count):base()
        {
            PortNum = count;
        }


        public Duplicator(Duplicator<rayT> other) :base(other)
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
            return new Duplicator<rayT>(this);
        }

        public override void OnPortUpdate(IPort inport)
        {
            if (PortNum == 0) return;
            RunRoutine(GetRoutine());
        }

        IEnumerator<Status> GetRoutine()
        {
            var refRay = inPort0.Ray;
            inPort0.Ray = null;

            outPorts[0].Ray = refRay;
            for (int i = 1; i < outPorts.Length; i++)
                outPorts[i].Ray = refRay?.RayClone();

            yield return Status.EmitAndShutdown;
        }
    }
}
