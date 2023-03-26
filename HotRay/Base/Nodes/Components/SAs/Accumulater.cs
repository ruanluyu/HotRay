using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.SAs
{
    /// <summary>
    /// Accumulate sequenced rays split by Sequencer
    /// </summary>
    /// <typeparam name="inRayT"></typeparam>
    /// <typeparam name="outRayT"></typeparam>
    public abstract class Accumulater<inRayT, outRayT> : OneOneComponent<inRayT, outRayT>
        where inRayT : RayBase
        where outRayT : RayBase
    {
        public Accumulater() : base() { }
        public Accumulater(Accumulater<inRayT, outRayT> other) : base(other) { }

        public async override Task<Status> OnActivated()
        {
            await base.OnActivated();
            if (!_running) RunRoutine(GetRoutine());
            return Status.Shutdown;
        }

        public override void Init()
        {
            base.Init();
            _running = false;
        }

        bool _running = false;

        async IAsyncEnumerator<Status> GetRoutine()
        {
            _running = true;
            try
            {
                while (true)
                {
                    await AccumulateBegin();
                    while (true)
                    {
                        if (inPort0.ChangedSinceLastCheck)
                        {
                            if (inPort0.Ray == null)
                                break;
                            await AccumulatePush((inPort0.Ray as inRayT)!);
                        }
                        yield return Status.WaitForNextStep;
                    }

                    yield return EmitRayTo(outPort0, await AccumulatePop(), true);

                    if (inPort0.Ray == null)
                    {
                        _running = false;
                        yield return Status.Shutdown;
                        yield break;
                    }
                }
            }
            finally
            {
                _running = false;
            }
        }



        /// <summary>
        /// Combine rays into one ray
        /// </summary>
        /// <param name="inRays"></param>
        /// <returns></returns>
        protected abstract Task AccumulateBegin();

        protected abstract Task AccumulatePush(inRayT rayT);

        protected abstract Task<outRayT?> AccumulatePop();

    }
}
