using HotRay.Base.Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components
{

    /// <summary>
    /// Multi-Ticks-Single-Task node base
    /// </summary>
    public abstract class MTSTComponentBase : ComponentBase
    {
        public MTSTComponentBase() : base()
        {
        }

        public MTSTComponentBase(MTSTComponentBase other) : base(other)
        {
        }

        Queue<Task<Status>>? queuedTasks = null;
        Queue<Task<Status>> QueuedTasks
        {
            get
            {
                if (queuedTasks == null) queuedTasks = new Queue<Task<Status>>();
                return queuedTasks;
            }
        }

        public abstract Task<Status> OnActivatedTask();

        public override sealed async Task<Status> OnActivated()
        {
            await base.OnActivated();
            var t = OnActivatedTask();

            if(t.IsCompleted)
            {
                return await t;
            }
            else
            {
                QueuedTasks.Enqueue(t);
            }
            RunRoutineIfNeeded();
            return Status.Shutdown;
        }


        IAsyncEnumerator<Status>? runningRoutine = null;
        void RunRoutineIfNeeded()
        {
            if (runningRoutine != null) return;
            if (QueuedTasks.Count > 0) return;
            runningRoutine = GetRoutine();
            RunRoutine(runningRoutine);
        }

        async IAsyncEnumerator<Status> GetRoutine()
        {
            try
            {
                while(QueuedTasks.TryPeek(out var t))
                {
                    if(t.IsCompleted)
                    {
                        yield return await QueuedTasks.Dequeue();
                    }
                    else
                    {
                        yield return Status.WaitForNextStep;
                    }
                }
                runningRoutine = null;
            }
            finally
            {
                runningRoutine = null;
            }
            yield return Status.Shutdown;
        }


    }
}
