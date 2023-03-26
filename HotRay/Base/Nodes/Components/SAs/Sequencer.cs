using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.SAs
{
    /// <summary>
    /// Split one ray to sequenced rays. 
    /// </summary>
    /// <typeparam name="inRayT"></typeparam>
    /// <typeparam name="outRayT"></typeparam>
    public abstract class Sequencer<inRayT, outRayT> : OneOneComponent<inRayT, outRayT> where inRayT : RayBase where outRayT : RayBase
    {
        public Sequencer() : base() { }
        public Sequencer(Sequencer<inRayT, outRayT> other) : base(other) { }

        Queue<inRayT?>? cacheQueue;
        Queue<inRayT?> CacheQueue
        {
            get
            {
                if (cacheQueue == null) cacheQueue = new Queue<inRayT?>();
                return cacheQueue;
            }
        }

        public override void Init()
        {
            base.Init();
            cacheQueue = null;
            _running = false;
        }

        public async override Task<Status> OnActivated()
        {
            await base.OnActivated();
            CacheQueue.Enqueue(inPort0.Ray as inRayT);
            if (!_running) RunRoutine(GetRoutine());
            return Status.Shutdown;
        }

        protected abstract IAsyncEnumerator<outRayT?> ApplySplit(inRayT? inRay);

        bool _running = false;
        async IAsyncEnumerator<Status> GetRoutine()
        {
            _running = true;
            try
            {
                while (CacheQueue.Count > 0)
                {
                    var e = ApplySplit(CacheQueue.Dequeue());

                    while (await e.MoveNextAsync())
                        yield return EmitRayTo(outPort0, e.Current, true);

                    if (outPort0.Ray != null)
                        yield return EmitRayTo(outPort0, null, true);
                }
            }
            finally
            {
                _running = false;
            }
            yield return Status.Shutdown;
        }


    }
}
