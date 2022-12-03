using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class StringRay : RayBase<string>
    {

        public override object Clone()
        {
            return new StringRay() { Data = Data };
        }

    }
}
