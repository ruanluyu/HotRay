using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotRay.Base.Ray.Hot;
using HotRay.Base.Ray.Lite;

namespace HotRay.Base.Ray
{
    public class SignalRay : RayBase
    {
        public static readonly SignalRay SharedSignal = new();

        public SignalRay() : base() { }
        public SignalRay(SignalRay other) : base(other) { }


        public override RayBase RayClone() => SharedSignal;

        
        
        public override RayBase? CastTo(Type targetType)
        {
            if(targetType == typeof(SignalRay)) return SharedSignal;
            if (targetType == typeof(ObjectRay)) return new ObjectRay() { Data = null };
            if (targetType == typeof(StringRay)) return new StringRay() { Data = ToString() };
            return base.CastTo(targetType);
        }


        public override string ToString()
        {
            return "signal";
        }
    }
}
