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
        public RandomPulseSource():base() { }
        public RandomPulseSource(RandomPulseSource other) :base(other) { EmitProbability = other.EmitProbability; RandomSeed = other.RandomSeed; }

        public RandomPulseSource(float prob, int seed) : base() { EmitProbability = prob; RandomSeed = seed; }

        public float EmitProbability { set; get; }
        public int RandomSeed { set; get; }

        public override INode CloneNode()
        {
            return new RandomPulseSource(this);
        }

        public override void OnEntry()
        {
            RunRoutine(GetRoutine());
        }

        IEnumerator<Status> GetRoutine()
        {
            var radomStatus = new Random(RandomSeed);
            while(true)
            {
                outPort0.Ray = radomStatus.NextSingle() <= EmitProbability ? SignalRay.SharedSignal : null;
                yield return Status.EmitAndWaitForNextStep;
            }
        }

    }
}
