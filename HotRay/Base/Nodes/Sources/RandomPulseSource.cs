using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public class RandomPulseSource : OneRaySource<SignalRay>
    {
        public RandomPulseSource():base() { EmitProbability = 0.5f; }
        public RandomPulseSource(RandomPulseSource other) :base(other) { EmitProbability = other.EmitProbability; RandomSeed = other.RandomSeed; }

        public RandomPulseSource(float prob, int seed) : base() { EmitProbability = prob; RandomSeed = seed; }

        public float EmitProbability { set; get; }
        public int RandomSeed { set; get; }

        public override NodeBase CloneNode()
        {
            return new RandomPulseSource(this);
        }

        public override Status OnEntry()
        {
            if(EmitProbability <= 0) return Status.Shutdown;
            RunRoutine(GetRoutine());
            return Status.Shutdown;
        }

        IEnumerator<Status> GetRoutine()
        {
            var radomStatus = new Random(RandomSeed);
            var lastSignal = false;
            while(true)
            {
                var curSignal = radomStatus.NextSingle() <= EmitProbability;
                if(curSignal != lastSignal)
                {
                    outPort0.Ray = curSignal ? SignalRay.SharedSignal : null;
                    lastSignal = curSignal;
                    yield return Status.WaitForNextStepAndEmit;
                }
                else
                {
                    yield return Status.WaitForNextStep;
                }
            }
        }

    }
}
