using HotRay.Base.Nodes.Sources;
using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{
    public abstract class ComponentBase : NodeBase
    {
        public ComponentBase() : base()
        {
            inPortList = SharedEmptyInPorts;
            outPortList = SharedEmptyOutPorts;
        }

        public ComponentBase(ComponentBase other) : base(other)
        {
            inPortList = SharedEmptyInPorts;
            outPortList = SharedEmptyOutPorts;
        }

        protected InPort[] inPortList;
        protected OutPort[] outPortList;

        protected void ResetInPortNum<rayT>(int num) where rayT : RayBase
        {
            if (num == inPortList.Length) return;

            var oldList = inPortList;
            inPortList = new InPort[num];
            for (int i = 0; i < num; i++)
            {
                if (i < oldList.Length)
                {
                    inPortList[i] = oldList[i];
                }
                else
                {
                    inPortList[i] = CreateInPort<rayT>();
                }
            }

            for (int i = inPortList.Length; i < oldList.Length; i++)
            {
                var p = oldList[i];
                if (p != null && p.SourcePort != null)
                    p.SourcePort.BreakConnection();
            }
        }

        protected void ResetOutPortNum<rayT>(int num) where rayT : RayBase
        {
            if (num == outPortList.Length) return;

            var oldList = outPortList;
            outPortList = new OutPort[num];
            for (int i = 0; i < num; i++)
            {
                if (i < oldList.Length)
                {
                    outPortList[i] = oldList[i];
                }
                else
                {
                    outPortList[i] = CreateOutPort<rayT>();
                }
            }

            for (int i = inPortList.Length; i < oldList.Length; i++)
            {
                var p = oldList[i];
                if (p != null)
                    p.BreakConnection();
            }
        }


        public override IReadOnlyList<InPort> InPorts => inPortList;

        public override IReadOnlyList<OutPort> OutPorts => outPortList;

        public override Status OnEntry() { return Status.Shutdown; }

    }
}
