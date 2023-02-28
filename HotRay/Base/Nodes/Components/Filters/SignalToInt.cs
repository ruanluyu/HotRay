using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Filters
{
    public class SignalToInt : FilterBase<SignalRay, IntRay>
    {
        public SignalToInt() : base()
        {

        }

        public SignalToInt(SignalToInt other) : base(other)
        {
            EmitValue = other.EmitValue;
        }

        public int EmitValue { get; set; }

        public override NodeBase CloneNode()
        {
            return new SignalToInt(this);
        }

        protected override Task<IntRay?> ParseRayType(SignalRay? inR)
        {
            if (inR == null) return Task.FromResult<IntRay?>(null);
            return Task.FromResult<IntRay?>(new IntRay() { Data = EmitValue });
        }
    }
}
