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
        public rayT? EmitValue { get => exposedParameters.emitValue; set => exposedParameters.emitValue =value; }

        private struct Parameters
        {
            public rayT emitValue;
        }

        Parameters exposedParameters, cachedParameters;


        public ConstantSource() : base()
        {

        }

        public ConstantSource(ConstantSource<rayT> other) : base(other)
        {

        }

        public override NodeBase CloneNode()
        {
            return new ConstantSource<rayT>(this);
        }


        public override Status OnEntry()
        {
            base.OnEntry();
            EmitRayTo(outPort0, cachedParameters.emitValue);
            return Status.Shutdown;
        }
    }
}
