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

        protected override IntRay ParseRayType(SignalRay inR)
        {
            return new IntRay() { Data = EmitValue };
        }
    }
}
