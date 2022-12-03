using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class FloatRay : RayBase<double>
    {
        public override object Clone()
        {
            return new FloatRay() { Data = Data };
        }
    }
}
