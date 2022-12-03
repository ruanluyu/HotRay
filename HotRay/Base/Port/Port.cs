using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    public class Port: BaseObject, IPort
    {
        public virtual RayBase? Ray
        {
            get; set;
        }
        
    }
    public class Port<rayT> : Port where rayT : RayBase
    {
        public Type RayType => typeof(rayT);
    }
}
