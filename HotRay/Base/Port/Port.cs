using HotRay.Base.Ray;
using HotRay.Base.Ray.Lite;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    
    public class Port<rayT> : PortBase where rayT : RayBase
    {

        public Port() { }
        public Port(Port<rayT> other) : base(other) 
        {

        }

        public override bool ConnectableTo(IPort? targetPort)
        {
            if (targetPort == null) return false;
            if (targetPort is Port<SignalRay>) return true;
            
            var t = typeof(rayT);
            var tpType = targetPort.GetType();

            if (typeof(Port<rayT>) == tpType) return true;

            if (tpType.GetGenericTypeDefinition() != typeof(Port<>)) return false;
            var ot = tpType.GetGenericArguments()[0];

            if (t == ot) return true;
            if (ot.IsInstanceOfType(this)) return true;

            return false;
        }

        public override void SendRay()
        {
            if (TargetPorts.Count == 0) return;
            if (Ray == null) return;
            var cRay = Ray;
            Ray = null;
            _SendTo(TargetPorts[0], cRay, false);
            for (int i = 1; i < TargetPorts.Count(); i++)
            {
                _SendTo(TargetPorts[i], cRay, true);
            }
            
        }

        void _SendTo(IPort targetPort, RayBase ray, bool clone)
        {
            if (targetPort == null) return;
            if (targetPort is Port<SignalRay> sp)
            {
                sp.Ray = SignalRay.SharedSignal;
            }
            else
            {
                targetPort.Ray = clone ? (ray.Clone() as RayBase) : ray;
            }
        }
    }
}
