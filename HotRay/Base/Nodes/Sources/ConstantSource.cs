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
        public rayT? EmitValue { get => p.emitValue; set => p.emitValue = value; }

        private struct Parameters
        {
            public rayT? emitValue;
        }

        Parameters p;


        public ConstantSource() : base()
        {
            EmitValue = null;
        }

        public ConstantSource(ConstantSource<rayT> other) : base(other)
        {
            p = other.p;
        }

        /*public override void OnCacheParameters()
        {
            base.OnCacheParameters();
            cached = exposed;
        }*/

        public override NodeBase CloneNode()
        {
            return new ConstantSource<rayT>(this);
        }


        public override async Task<Status> OnStart()
        {
            await base.OnStart();
            EmitRayTo(outPort0, p.emitValue);
            return Status.Shutdown;
        }
    }
}
