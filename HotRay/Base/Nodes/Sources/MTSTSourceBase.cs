using HotRay.Base.Port;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public abstract class MTSTSourceBase:SourceBase
    {
        public MTSTSourceBase():base()
        {

        }

        public MTSTSourceBase(MTSTSourceBase other):base(other)
        {

        }

        public abstract IAsyncEnumerator<Status> OnBigBangTask();


        public override sealed async Task<Status> OnStart()
        {
            await base.OnStart();

            RunRoutine(GetRoutine());

            return Status.Shutdown;
        }


        async IAsyncEnumerator<Status> GetRoutine()
        {
            var t = OnBigBangTask();
            
            while(true)
            {
                var task = t.MoveNextAsync();
                while (true)
                {
                    if(task.IsCompleted)
                    {
                        if(task.Result)
                        {
                            yield return t.Current;
                            break;
                        }
                        else
                        {
                            Log("Warnning: Use 'yield return Status.Shutdown' to close the routine instead of 'yield break'. ");
                            yield return Status.Shutdown;
                        }
                    }
                    else
                    {
                        yield return Status.WaitForNextStep;
                    }
                }
            }
        }
    }
}
