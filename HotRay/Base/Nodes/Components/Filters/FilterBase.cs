using HotRay.Base.Nodes.Components.Utils;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HotRay.Base.Nodes.INode;

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

        protected abstract outRay ParseRayType(inRay inR);

        public sealed override IEnumerator<Status> GetRoutine()
        {
            if (inPort0.Ray == null) yield return Status.Shutdown;

            outPort0.Ray = ParseRayType((inPort0.Ray as inRay)!);
            inPort0.Ray = null;
            yield return Status.EmitAndShutdown;
        }
    }
}
