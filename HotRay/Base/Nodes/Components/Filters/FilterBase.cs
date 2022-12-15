using HotRay.Base.Nodes.Components.Utils;
using HotRay.Base.Port;
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

        protected abstract outRay? ParseRayType(inRay? inR);

        public sealed override Status OnPortUpdate(IPort inport)
        {
            if(inport.Ray is inRay inray)
            {
                var outray = ParseRayType(inray);
                if(outray == null)
                {
                    if (outPort0.Ray == null) return Status.Shutdown;
                    outPort0.Ray = null;
                    return Status.ShutdownAndEmit;
                }
                else
                {
                    outPort0.Ray = outray;
                    return Status.ShutdownAndEmit;
                }
            }
            else
            {
                if (outPort0.Ray == null) return Status.Shutdown;
                outPort0.Ray = null;
                return Status.ShutdownAndEmit;
            }
        }

    }
}
