using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public class FloatRay : LiteRayBase<double>
    {
        public FloatRay() : base() { }
        public FloatRay(FloatRay other) : base(other) { }

        public override RayBase RayClone()
        {
            return new FloatRay(this);
        }
        public override string ToString()
        {
            return Data.ToString();
        }
    }
}
