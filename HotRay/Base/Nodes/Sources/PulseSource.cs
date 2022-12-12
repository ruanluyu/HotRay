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

        public PulseSource() : base()
        {
            Count = 1;
        }

        public PulseSource(PulseSource other) : base(other)
        {
            FirstDelay = other.FirstDelay;
            Count = other.Count;
            Interval = other.Interval;
        }


        /// <summary>
        /// Delay before emit the first signal. 
        /// </summary>
        public int FirstDelay
        {
            get; set;
        }

        /// <summary>
        /// Greater than 0: Emission limit exists; Equals to 0: No emission; Less than 0: No emission limit. 
        /// </summary>
        public int Count
        {
            get; set;
        }

        /// <summary>
        /// Greater than 0: Interval exists, Less or equals to 0: No interval
        /// </summary>
        public int Interval
        {
            get;set;
        }

        public override INode CloneNode()
        {
            return new PulseSource(this);
        }

        public override Status OnEntry()
        {
            if (Count == 0) return Status.Shutdown;
            RunRoutine(GetRoutine());
            return Status.Shutdown;
        }

        IEnumerator<Status> GetRoutine()
        {
            int c = Count;
            if (c == 0) yield return Status.Shutdown;

            int it = Interval;
            int fd = FirstDelay;

            while (fd-- > 0) yield return Status.WaitForNextStep;

            if(it > 0)
            {
                while (true)
                {
                    outPort0.Ray = SignalRay.SharedSignal;
                    yield return Status.EmitAndWaitForNextStep;
                    outPort0.Ray = null;
                    yield return Status.EmitAndWaitForNextStep;
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
                if (c < 0) yield return Status.EmitAndShutdown;
                yield return Status.EmitAndWaitForNextStep;
                for (int i = 0; i < c-1; i++)
                {
                    yield return Status.WaitForNextStep;
                }
                outPort0.Ray = null;
                yield return Status.EmitAndShutdown;
            }
        }

    }
}
