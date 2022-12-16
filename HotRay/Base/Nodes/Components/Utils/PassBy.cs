using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    /// <summary>
    /// in0: data, out0: data, out1 signal
    /// </summary>
    /// <typeparam name="rayT"></typeparam>
    public class PassBy<rayT>:OneTwoComponent<rayT,rayT,SignalRay>
        where rayT:RayBase
    {
        public PassBy() : base()
        {

        }

        public PassBy(PassBy<rayT> other) : base(other)
        {
        }

        public override NodeBase CloneNode()
        {
            return new PassBy<rayT>(this);
        }

        public override Status OnPortUpdate(PortBase inport)
        {
            if (inPort0.Ray == null)
            {
                outPort0.Ray = null;
                outPort1.Ray = null;
                return Status.ShutdownAndEmit;
            }
            else
            {
                outPort0.Ray = inPort0.Ray;
                outPort1.Ray = SignalRay.SharedSignal;
                return Status.ShutdownAndEmit;
            }
        }

        public virtual bool InsertAfter(PortBase outport)
        {
            var t = outport.TargetPort;

            if (!outport.ConnectableTo(inPort0)) return false;
            outport.ConnectTo(inPort0);

            if(t != null)
            {
                outPort0.ConnectTo(t);
            }

            return true;
        }
    }
}
