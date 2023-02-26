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


        public override RayBase? CastTo(Type targetType)
        {
            if (Data != null)
            {
                var dataType = Data.GetType();
                if(Data is RayBase rb)
                {
                    if (dataType == targetType || dataType.IsSubclassOf(targetType))
                        return rb;
                    return rb.CastTo(targetType);
                }
            }
            return base.CastTo(targetType);
        }


        public override RayBase RayClone()
        {
            return new ObjectRay(this);
        }


        private bool disposedValue;
        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    if(Data is IDisposable dp)
                    {
                        dp.Dispose();
                    }
                }
                disposedValue = true;
            }
            base.Dispose(disposing);
        }
    }
}
