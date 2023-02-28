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
        private struct Parameters
        {
            public float emitProbability;
            public int randomSeed;
        }

        Parameters p;


        public float EmitProbability
        {
            set => p.emitProbability = value;
            get => p.emitProbability;
        }
        public int RandomSeed
        {
            set => p.randomSeed = value;
            get => p.randomSeed;
        }

        public RandomPulseSource():base() { EmitProbability = 0.5f; RandomSeed = 0xfabf; }
        public RandomPulseSource(float prob, int seed) : base() { EmitProbability = prob; RandomSeed = seed; }
        public RandomPulseSource(RandomPulseSource other) :base(other) { p = other.p; }

        /*public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }*/

        public override NodeBase CloneNode()
        {
            return new RandomPulseSource(this);
        }

        public override Task<Status> OnStart()
        {
            if(p.emitProbability <= 0) return Status.ShutdownTask;
            RunRoutine(GetRoutine());
            return Status.ShutdownTask;
        }

        async IAsyncEnumerator<Status> GetRoutine()
        {
            var radomStatus = new Random(p.randomSeed);
            var lastSignal = false;
            while(true)
            {
                var curSignal = radomStatus.NextSingle() <= p.emitProbability;
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
