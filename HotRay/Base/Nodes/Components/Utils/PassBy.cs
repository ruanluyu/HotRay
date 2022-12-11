using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class PassBy<rayT>:OneTwoComponent<rayT,rayT,SignalRay>
        where rayT:RayBase
    {
        public PassBy() : base()
        {

        }

        public PassBy(PassBy<rayT> other) : base(other)
        {
        }

        public override INode CloneNode()
        {
            return new PassBy<rayT>(this);
        }

        public override void OnPortUpdate(IPort inport)
        {
            RunRoutine(GetRoutine());
        }

        IEnumerator<Status> GetRoutine()
        {
            if (inPort0.Ray == null)
            {
                outPort0.Ray = null;
                yield return Status.EmitAndShutdown;
            }
            else
            {
                outPort0.Ray = inPort0.Ray;
                inPort0.Ray = null;

                outPort1.Ray = SignalRay.SharedSignal;
                yield return Status.EmitAndShutdown;
            }
        }

    }
}
