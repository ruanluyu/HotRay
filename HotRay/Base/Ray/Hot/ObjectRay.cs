using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray.Hot
{
    public class ObjectRay : HotRayBase<object>
    {
        public ObjectRay():base() { }
        public ObjectRay(ObjectRay other):base(other) 
        {
            if (other is ICloneable c) Data = c.Clone();
            Data = other.Data;
        }



        public override IRay RayClone()
        {
            return new ObjectRay(this);
        }
    }
}
