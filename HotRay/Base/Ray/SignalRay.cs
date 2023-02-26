using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HotRay.Base.Ray.Lite;

namespace HotRay.Base.Ray
{
    public class SignalRay : RayBase
    {
        public static readonly SignalRay SharedSignal = new();

        public SignalRay() : base() { }
        public SignalRay(SignalRay other) : base(other) { }


        public override RayBase RayClone() => SharedSignal;


        //public static implicit operator SignalRay(DataRayBase other) => SharedSignal;

        public override string ToString()
        {
            return "signal";
        }
    }
}
