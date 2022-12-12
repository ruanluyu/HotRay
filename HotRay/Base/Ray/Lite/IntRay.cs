using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public class IntRay : LiteRayBase<long>
    {
        public IntRay():base() { }
        public IntRay(IntRay other):base(other) { }

        public override IRay RayClone()
        {
            return new IntRay(this);
        }

        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
