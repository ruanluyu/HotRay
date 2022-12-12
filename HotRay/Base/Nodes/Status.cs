using HotRay.Base.Port;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes
{
    /// <summary>
    /// yield return false: no result and need continuously run at next step. <para />
    /// yield return true: has result and need continuously run at next step. <para />
    /// yield break or quit: deactivates node, will be activated again when any of the in-ports recieved data. <para />
    /// </summary>
    public struct Status: IEquatable<Status>
    {
        public bool HasResult;
        public bool Finished;
        public IEnumerable<IPort>? PortMask;


        public static Status Shutdown => new Status() { HasResult = false, Finished = true, };
        public static Status WaitForNextStep => new Status() { HasResult = false, Finished = false, };
        public static Status EmitAndWaitForNextStep => new Status() { HasResult = true, Finished = false, };
        public static Status EmitAndShutdown => new Status() { HasResult = true, Finished = true, };

        public static Status EmitAndShutdownWith(params IPort[] portMask) => new Status() { HasResult = true, Finished = true, PortMask = portMask };
        
        public static Status EmitAndShutdownWith(IEnumerable<IPort> portMask) => new Status() { HasResult = true, Finished = true, PortMask = portMask };

        public static Status EmitAndWaitForNextStepWith(params IPort[] portMask) => new Status() { HasResult = true, Finished = false, PortMask = portMask };
        
        public static Status EmitAndWaitForNextStepWith(IEnumerable<IPort> portMask) => new Status() { HasResult = true, Finished = false, PortMask = portMask };


        public bool Equals(Status other)
        {
            return HasResult == other.HasResult && Finished == other.Finished;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Status other && this.Equals(other);

        public static bool operator ==(Status lhs, Status rhs) => lhs.Equals(rhs);
        public static bool operator !=(Status lhs, Status rhs) => !lhs.Equals(rhs);

        public override int GetHashCode()
        {
            if(HasResult)
            {
                if(Finished)
                {
                    return 0;
                }
                else
                {
                    return 1;
                }
            }
            else
            {
                if (Finished)
                {
                    return 2;
                }
                else
                {
                    return 3;
                }
            }
        }
    }
}
