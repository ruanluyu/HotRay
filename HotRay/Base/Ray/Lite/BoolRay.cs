using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Lite
{
    public class BoolRay : LiteRayBase<bool>
    {
        public BoolRay() : base() { }
        public BoolRay(BoolRay other) : base(other) { }


        public override RayBase RayClone()
        {
            return new BoolRay(this);
        }

        public override string ToString()
        {
            return Data.ToString();
        }

    }
}
