using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Logics
{
    public class AndGate : ComponentBase
    {
        protected readonly OutPort<SignalRay> outPort0;

        public AndGate() : this(2)
        {
        }

        public AndGate(int count) : base()
        {
            PortNum = count;
            outPort0 = CreateOutPort<SignalRay>();
            outPortList = new OutPort[] { outPort0 };
        }


        public AndGate(AndGate other) : base(other)
        {
            PortNum = other.PortNum;
            outPort0 = CreateOutPort<SignalRay>();
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
                ResetInPortNum<SignalRay>(value);
            }
        }

        public override NodeBase CloneNode()
        {
            return new AndGate(this);
        }

        public override Task<Status> OnActivated()
        {
            if (PortNum == 0) return Status.ShutdownTask;
            for (int i = 0; i < inPortList.Length; i++)
            {
                if (inPortList[i].Ray == null) return EmitSignalToTask(outPort0, false);
            }
            return EmitSignalToTask(outPort0, true);
        }

    }
}
