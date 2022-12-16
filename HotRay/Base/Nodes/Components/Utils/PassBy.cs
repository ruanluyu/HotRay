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
    /// Emit signal if inport received ray. 
    /// <br/><br/>
    /// in-0(rayT): data <br/>
    /// out-0(rayT): data<br/>
    /// out-1(SignalRay): signal
    /// </summary>
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

        public override Status OnActivated()
        {
            var res = EmitRayTo(outPort0, inPort0.Ray);
            if (res.HasResult) // Status changed
            {
                var res2 = EmitSignalTo(outPort1, inPort0.Ray != null);
                if (res2.HasResult) return Status.ShutdownAndEmit;
                return Status.ShutdownAndEmitWith(outPort0);
            }
            return Status.Shutdown;
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
