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
    /// Describe the status of the node. 
    /// </summary>
    public struct Status: IEquatable<Status>
    {
        public bool HasResult;
        public bool Finished;
        public IEnumerable<PortBase>? PortMask;

        /// <summary>
        /// No result, and shutdown the routine in this tick. 
        /// </summary>
        public static Status Shutdown => new Status() { HasResult = false, Finished = true, };

        /// <summary>
        /// No result, and this routine has works to do in the next tick.  
        /// </summary>
        public static Status WaitForNextStep => new Status() { HasResult = false, Finished = false, };

        /// <summary>
        /// Has result, and this routine has works to do in the next tick.  
        /// </summary>
        public static Status WaitForNextStepAndEmit => new Status() { HasResult = true, Finished = false, };

        /// <summary>
        /// Has result, and shutdown the routine in this tick. 
        /// </summary>
        public static Status ShutdownAndEmit => new Status() { HasResult = true, Finished = true, };

        /// <summary>
        /// Has result, and shutdown the routine in this tick. 
        /// </summary>
        /// <param name="portMask">The outports that hold results. </param>
        /// <returns></returns>
        public static Status ShutdownAndEmitWith(params PortBase[] portMask) => new Status() { HasResult = true, Finished = true, PortMask = portMask };

        /// <summary>
        /// Has result, and shutdown the routine in this tick. 
        /// </summary>
        /// <param name="portMask">The outports that hold results. </param>
        /// <returns></returns>
        public static Status ShutdownAndEmitWith(IEnumerable<PortBase> portMask) => new Status() { HasResult = true, Finished = true, PortMask = portMask };

        /// <summary>
        /// Has result, and this routine has works to do in the next tick.  
        /// </summary>
        /// <param name="portMask">The outports that hold results. </param>
        /// <returns></returns>
        public static Status WaitForNextStepAndEmitWith(params PortBase[] portMask) => new Status() { HasResult = true, Finished = false, PortMask = portMask };

        /// <summary>
        /// Has result, and this routine has works to do in the next tick.  
        /// </summary>
        /// <param name="portMask">The outports that hold results. </param>
        /// <returns></returns>
        public static Status WaitForNextStepAndEmitWith(IEnumerable<PortBase> portMask) => new Status() { HasResult = true, Finished = false, PortMask = portMask };


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
