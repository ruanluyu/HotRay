using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Utils
{
    public class Delayer<rayT> : OneOneComponent<rayT, rayT> where rayT : RayBase
    {
        public Delayer() : base()
        {
            Delay = 1;
        }

        public Delayer(Delayer<rayT> other) : base(other)
        {
            Delay = other.Delay;
        }


        public virtual int Delay
        {
            get; set;
        }

        public override NodeBase CloneNode()
        {
            return new Delayer<rayT>(this);
        }

        public override Status OnActivated()
        {
            if(inPort0.ChangedSinceLastCheck)
            {
                if (Delay <= 1)
                {
                    return EmitRayTo(outPort0, inPort0.Ray);
                }
                else
                {
                    RunRoutine(GetRoutine(inPort0.Ray));
                }
            }
            return Status.Shutdown;
        }

        IEnumerator<Status> GetRoutine(RayBase? outputRay)
        {
            int d = Delay - 1;
            while (--d >= 0) yield return Status.WaitForNextStep;
            yield return EmitRayTo(outPort0, outputRay);
        }

    }
}
