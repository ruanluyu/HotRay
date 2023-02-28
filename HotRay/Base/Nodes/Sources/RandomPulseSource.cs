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

        Parameters exposed, cached;


        public float EmitProbability
        {
            set => exposed.emitProbability = value;
            get => exposed.emitProbability;
        }
        public int RandomSeed
        {
            set => exposed.randomSeed = value;
            get => exposed.randomSeed;
        }

        public RandomPulseSource():base() { EmitProbability = 0.5f; RandomSeed = 0xfabf; }
        public RandomPulseSource(float prob, int seed) : base() { EmitProbability = prob; RandomSeed = seed; }
        public RandomPulseSource(RandomPulseSource other) :base(other) { exposed = other.exposed; }

        public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }

        public override NodeBase CloneNode()
        {
            return new RandomPulseSource(this);
        }

        public override Task<Status> OnBigBang()
        {
            if(cached.emitProbability <= 0) return Status.ShutdownTask;
            RunRoutine(GetRoutine());
            return Status.ShutdownTask;
        }

        async IAsyncEnumerator<Status> GetRoutine()
        {
            var radomStatus = new Random(cached.randomSeed);
            var lastSignal = false;
            while(true)
            {
                var curSignal = radomStatus.NextSingle() <= cached.emitProbability;
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
