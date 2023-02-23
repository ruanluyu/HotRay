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
        protected readonly OutPort<rayT> outPort0;

        public Merge() : this(2)
        {

        }

        public Merge(int count) : base()
        {
            outPort0 = CreateOutPort<rayT>();
            PortNum = count;
            outPortList = new OutPort[] { outPort0 };
        }


        public Merge(Merge<rayT> other) : base(other)
        {
            outPort0 = CreateOutPort<rayT>();
            PortNum = other.PortNum;
            outPortList = new OutPort[] { outPort0 };
        }

        public int PortNum
        {
            get => inPortList.Length;
            set
            {
                if (value < 0)
                {
                    PortNum = 0;
                    return;
                }
                ResetInPortNum<rayT>(value);
            }
        }

        public override NodeBase CloneNode()
        {
            return new Merge<rayT>(this);
        }


        public override Status OnActivated()
        {
            if (PortNum == 0) return Status.Shutdown;
            for (int i = 0; i < inPortList.Length; i++)
            {
                var port = inPortList[i];
                if (port.ChangedSinceLastCheck && port.Ray != null)
                {
                    for (int j = i + 1; j < inPortList.Length; j++)
                    {
                        port.ResetChanged();
                    }
                    return EmitRayTo(outPort0, port.Ray);
                }
            }
            return EmitRayTo(outPort0, null);
        }

    }
}
