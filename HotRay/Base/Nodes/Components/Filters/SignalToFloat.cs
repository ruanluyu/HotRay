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

        protected override FloatRay ParseRayType(SignalRay inR)
        {
            return new FloatRay() { Data = EmitValue };
        }
    }
}
