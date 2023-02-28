using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Filters
{
    public class SignalToFloat : FilterBase<SignalRay, FloatRay>
    {
        public SignalToFloat() : base()
        {

        }

        public SignalToFloat(SignalToFloat other) : base(other)
        {
            EmitValue = other.EmitValue;
        }

        public double EmitValue { get; set; }

        public override NodeBase CloneNode()
        {
            return new SignalToFloat(this);
        }

        protected override Task<FloatRay?> ParseRayType(SignalRay? inR)
        {
            if (inR == null) return Task.FromResult<FloatRay?>(null);
            return Task.FromResult<FloatRay?>(new FloatRay() { Data = EmitValue });
        }
    }
}
