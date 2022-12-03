using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes
{
    public struct Status
    {
        public bool HasResult;
        public bool Finished;
        public static Status Shutdown => new Status() { HasResult = false, Finished = true, };
        public static Status WaitForNextStep => new Status() { HasResult = false, Finished = false, };
        public static Status EmitAndWaitForNextStep => new Status() { HasResult = true, Finished = false, };
        public static Status EmitAndShutdown => new Status() { HasResult = true, Finished = true, };
    }
}
