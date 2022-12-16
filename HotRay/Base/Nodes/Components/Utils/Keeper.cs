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
    /// Keep signal status. 
    /// <br/><br/>
    /// in-0(rayT): value. <br/>
    /// in-1(SignalRay): set.  <br/>
    /// out-0(rayT): kept value. 
    /// </summary>
    public class Keeper<rayT> : TwoOneComponent<rayT, SignalRay, rayT>
        where rayT: RayBase
    {

        public Keeper() : base() { }
        
        public Keeper(Keeper<rayT> other) : base(other) { }

        public override Status OnActivated()
        {
            if (inPort1.Ray == null)
            {
                // Keep
            }
            else
            {
                // Set
                return EmitRayTo(outPort0, inPort0.Ray);
            }
            return Status.Shutdown;
        }

        public override NodeBase CloneNode()
        {
            return new Keeper<rayT>(this);
        }
    }
}
