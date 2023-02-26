using HotRay.Base.Nodes.Components.Containers;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    public abstract class OutPort : PortBase
    {
        public OutPort() { }
        public OutPort(OutPort other) : base(other)
        {
            TargetPort = null;
        }

        public InPort? TargetPort { get; protected set; }

        bool IsSameBoxWith([NotNull] InPort targetPort)
        {
            return Box.GetParentBoxOf(this) == Box.GetParentBoxOf(targetPort);
        }

        static bool CanConvert(Type fromType, Type toType)
        {
            try
            {
                // Throws an exception if there is no conversion from fromType to toType
                Expression.Convert(Expression.Parameter(fromType), toType);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public virtual bool ConnectableTo([NotNull] InPort targetPort)
        {
            var tt = targetPort.RayType;
            var mt = RayType;
            if(IsSameBoxWith(targetPort))
            {
                if (mt == tt) return true;
                if (tt == typeof(SignalRay)) return true;
                if (CanConvert(mt, tt)) return true;
            }
            return false;
        }

        /// <summary>
        /// Will auto disconnect the Source port of targetPort
        /// </summary>
        /// <param name="targetPort"></param>
        public void ConnectTo([NotNull] InPort targetPort)
        {
            if (TargetPort != targetPort)
            {
                if (ConnectableTo(targetPort))
                {
                    if (targetPort.SourcePort != null)
                    {
                        targetPort.SourcePort.BreakConnection();
                    }
                    if(TargetPort != null)
                    {
                        BreakConnection();
                    }
                    TargetPort = targetPort;
                    targetPort.SourcePort = this;
                }
                else
                {
                    Log($"Failed to connect to: {targetPort}");
                }
            }
        }

        public void BreakConnection()
        {
            if (TargetPort == null) return;
            if (TargetPort.SourcePort == this) TargetPort.SourcePort = null;
            TargetPort = null;
        }

        public virtual void SendRay()
        {
            if (TargetPort != null)
            {
                // Log($"==[[{Ray?.ToString() ?? "null"}]]==> {TargetPort.Parent}");
                TargetPort.Ray = Ray;
            }
        }

    }

    public class OutPort<rayT> : OutPort where rayT : RayBase
    {
        public OutPort() { }
        public OutPort(OutPort<rayT> other) : base(other)
        {
            TargetPort = null;
        }

        public override Type RayType => typeof(rayT);

        public override PortBase ClonePort()
        {
            var np = new OutPort<rayT>(this);
            return np;
        }
        
    }
}
