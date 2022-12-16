using HotRay.Base.Nodes.Components.Utils;
using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotRay.Base.Nodes.NodeBase;

namespace HotRay.Base.Nodes.Components.Filters
{
    public abstract class FilterBase<inRay, outRay> : OneOneComponent<inRay, outRay>
        where inRay : RayBase
        where outRay : RayBase
    {
        public FilterBase() : base()
        {

        }

        public FilterBase(FilterBase<inRay, outRay> other) : base(other)
        {
        }

        protected abstract outRay? ParseRayType(inRay? inR);

        public sealed override Status OnPortUpdate(PortBase inport)
        {
            if(inport.Ray is inRay inray)
            {
                return EmitRayTo(outPort0, ParseRayType(inray));
            }
            else
            {
                return EmitRayTo(outPort0, null);
            }
        }

    }
}
