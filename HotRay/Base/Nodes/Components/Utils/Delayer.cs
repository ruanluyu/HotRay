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

        public override IEnumerator<Status> GetRoutine()
        {
            if (inPort0.Ray == null) yield return Status.Shutdown;

            int d = Delay;
            var inRay = inPort0.Ray;
            inPort0.Ray = null;

            while (--d >= 0)
                yield return Status.WaitForNextStep;
            
            outPort0.Ray = inRay;
            yield return Status.EmitAndShutdown;
        }

    }
}
