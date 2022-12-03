using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes
{
    public struct Status: IEquatable<Status>
    {
        public bool HasResult;
        public bool Finished;

        public static Status Shutdown => new Status() { HasResult = false, Finished = true, };
        public static Status WaitForNextStep => new Status() { HasResult = false, Finished = false, };
        public static Status EmitAndWaitForNextStep => new Status() { HasResult = true, Finished = false, };
        public static Status EmitAndShutdown => new Status() { HasResult = true, Finished = true, };

        public bool Equals(Status other)
        {
            return HasResult == other.HasResult && Finished == other.Finished;
        }

        public override bool Equals([NotNullWhen(true)] object? obj) => obj is Status other && this.Equals(other);

        public static bool operator ==(Status lhs, Status rhs) => lhs.Equals(rhs);
        public static bool operator !=(Status lhs, Status rhs) => !lhs.Equals(rhs);
    }
}
