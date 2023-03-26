using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Components.Filters
{
    public class SignalToString:FilterBase<SignalRay, StringRay>
    {
        public SignalToString() : base()
        {

        }

        public SignalToString(SignalToString other) : base(other)
        {
            EmitValue = other.EmitValue;
        }

        public string? EmitValue { get; set; }

        public override NodeBase CloneNode()
        {
            return new SignalToString(this);
        }

        protected override Task<StringRay?> ParseRayType(SignalRay? inR)
        {
            if (inR == null) return Task.FromResult<StringRay?>(null);
            return Task.FromResult<StringRay?>(new StringRay() { Data = EmitValue });
        }
    }
}
