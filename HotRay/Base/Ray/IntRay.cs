using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class IntRay : RayBase<long>
    {



        public override object Clone()
        {
            return new IntRay() { Data = Data };
        }

    }
}
