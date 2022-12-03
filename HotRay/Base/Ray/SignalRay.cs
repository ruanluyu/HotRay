using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Ray
{
    public class SignalRay : RayBase
    {
        public static readonly SignalRay SharedSignal = new();


        public override object Clone() => SharedSignal;
    }
}
