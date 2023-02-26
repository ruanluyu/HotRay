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
    }
}
