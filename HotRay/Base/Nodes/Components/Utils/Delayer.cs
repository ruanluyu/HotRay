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
        private struct Parameters
        {
            public int delay;
        }

        Parameters p;

        public Delayer() : base()
        {
            Delay = 1;
        }

        public Delayer(Delayer<rayT> other) : base(other)
        {
            p = other.p;
        }


        public virtual int Delay
        {
            get => p.delay; set => p.delay = value;
        }

        /*public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }*/

        public override NodeBase CloneNode()
        {
            return new Delayer<rayT>(this);
        }

        public override Task<Status> OnActivated()
        {
            if(inPort0.ChangedSinceLastCheck)
            {
                if (p.delay <= 1)
                {
                    return EmitRayToTask(outPort0, inPort0.Ray);
                }
                else
                {
                    RunRoutine(GetRoutine(inPort0.Ray));
                }
            }
            return Status.ShutdownTask;
        }

        async IAsyncEnumerator<Status> GetRoutine(RayBase? outputRay)
        {
            int d = p.delay - 1;
            while (--d >= 0) yield return Status.WaitForNextStep;
            yield return EmitRayTo(outPort0, outputRay);
        }

    }
}
