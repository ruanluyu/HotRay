using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotRay.Base.Ray.Lite;

namespace HotRay.Base.Ray
{
    public sealed class SignalRay : RayBase
    {
        public static readonly SignalRay SharedSignal = new();


        public override IRay RayClone() => SharedSignal;


    }
}
