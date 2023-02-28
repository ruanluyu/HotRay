using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Nodes.Sources
{
    public class ConstantSource<rayT> : OneRaySource<rayT> where rayT : RayBase
    {
        public rayT? EmitValue { get => exposed.emitValue; set => exposed.emitValue = value; }

        private struct Parameters
        {
            public rayT? emitValue;
        }

        Parameters exposed, cached;


        public ConstantSource() : base()
        {
            EmitValue = null;
        }

        public ConstantSource(ConstantSource<rayT> other) : base(other)
        {
            exposed = other.exposed;
        }

        public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }

        public override NodeBase CloneNode()
        {
            return new ConstantSource<rayT>(this);
        }


        public override async Task<Status> OnBigBang()
        {
            await base.OnBigBang();
            EmitRayTo(outPort0, cached.emitValue);
            return Status.Shutdown;
        }
    }
}
