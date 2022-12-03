using HotRay.Base.Ray.Lite;
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

        public override IEnumerator<Status> GetRoutine()
        {
            int c = Count;
            if (c == 0) yield return Status.Shutdown;

            int it = Interval;
            int fd = FirstDelay;

            while (fd-- > 0) yield return Status.WaitForNextStep;

            while (true)
            {
                outPort0.Ray = SignalRay.SharedSignal;
                yield return Status.EmitAndWaitForNextStep;
                outPort0.Ray = null;
                if (c > 0)
                {
                    --c;
                    if (c == 0)
                        yield break;
                }
                for (int i = 0; i < it; i++)
                    yield return Status.WaitForNextStep;
            }
        }

    }
}
