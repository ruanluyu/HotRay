using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public class PulseSource: OneRaySource<SignalRay>
    {

        /// <summary>
        /// Delay before emit the first signal. 
        /// </summary>
        public int FirstDelay
        {
            get=>p.firstDelay; 
            set=>p.firstDelay = value;
        }

        /// <summary>
        /// >0: Emission limit exists; <br/> 
        /// 0: Turn off; <br/>
        /// -1: No emission limit. 
        /// </summary>
        public int Count
        {
            get => p.count;
            set => p.count = value;
        }

        /// <summary>
        /// >0: Intervals exist; <br/>
        /// otherwise: No interval. 
        /// </summary>
        public int Interval
        {
            get => p.interval;
            set => p.interval = value;
        }


        private struct Parameters
        {
            public int firstDelay;
            public int count;
            public int interval;
        }

        Parameters p;

        public PulseSource() : base()
        {
            FirstDelay = 0;
            Count = 1;
            Interval = 1;
        }

        public PulseSource(PulseSource other) : base(other)
        {
            p = other.p;
        }

        

        public override NodeBase CloneNode()
        {
            return new PulseSource(this);
        }

        public override Task<Status> OnStart()
        {
            if (Count == 0) return Status.ShutdownTask;
            RunRoutine(GetRoutine());
            return Status.ShutdownTask;
        }

        /*public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }*/

        async IAsyncEnumerator<Status> GetRoutine()
        {
            int c = p.count;
            if (c == 0) yield return Status.Shutdown;

            int it = p.interval;
            int fd = p.firstDelay;

            while (fd-- > 0) yield return Status.WaitForNextStep;

            if(it > 0)
            {
                while (true)
                {
                    outPort0.Ray = SignalRay.SharedSignal;
                    yield return Status.WaitForNextStepAndEmit;
                    outPort0.Ray = null;
                    yield return Status.WaitForNextStepAndEmit;
                    if (c > 0)
                    {
                        --c;
                        if (c == 0)
                            yield return Status.Shutdown;
                    }
                    for (int i = 0; i < it - 1; i++)
                        yield return Status.WaitForNextStep;
                }
            }
            else
            {
                outPort0.Ray = SignalRay.SharedSignal;
                if (c < 0) yield return Status.ShutdownAndEmit;
                yield return Status.WaitForNextStepAndEmit;
                for (int i = 0; i < c-1; i++)
                {
                    yield return Status.WaitForNextStep;
                }
                outPort0.Ray = null;
                yield return Status.ShutdownAndEmit;
            }
        }

    }
}
