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

        public override INode CloneNode()
        {
            return new Delayer<rayT>(this);
        }

        public override Status OnPortUpdate(IPort inport)
        {
            
            if(Delay == 0)
            {
                outPort0.Ray = inport.Ray;
                return Status.EmitAndShutdown;
            }
            else
            {
                RunRoutine(GetRoutine(inport.Ray));
            }
            return Status.Shutdown;
        }

        IEnumerator<Status> GetRoutine(IRay? outputRay)
        {
            int d = Delay;
            while (--d >= 0)
            {
                yield return Status.WaitForNextStep;
            }
            outPort0.Ray = outputRay;
            yield return Status.EmitAndShutdown;
        }

    }
}
