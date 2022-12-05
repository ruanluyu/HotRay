using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{

    public class Port<rayT> : PortBase where rayT : IRay
    {

        public Port() { }
        public Port(Port<rayT> other) : base(other) 
        {

        }

        public override bool ConnectableTo([NotNull]IPort targetPort)
        {
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


    }
}
