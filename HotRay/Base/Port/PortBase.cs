using HotRay.Base.Nodes;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    public abstract class PortBase : BaseObject
    {
        public PortBase() { }
        public PortBase(PortBase other) : base(other) 
        {
            if(other.SourcePort != null)
            {
                other.SourcePort.ConnectTo(this);
            }
        }



        public PortBase? SourcePort { get; protected set; }

        public PortBase? TargetPort { get; protected set; }

        RayBase? _ray;
        bool _changed;


        public bool RayChangedReadOnly => _changed;

        /// <summary>
        /// This method only returns true one time if Ray changed. <br/>
        /// i.e.:<br/>
        /// RayChanged(); // True<br/>
        /// RayChanged(); // False<br/>
        /// RayChanged(); // False<br/>
        /// ...
        /// </summary>
        public bool RayChanged()
        {
            if(_changed)
            {
                _changed = false;
                return true;
            }
            return false;
        }


        public virtual RayBase? Ray
        {
            set
            {
                _changed = _ray != value;
                _ray = value;
            }
            get
            {
                return _ray;
            }
        }

        public abstract Type RayType { get; }

        public virtual void SendRay()
        {
            if (TargetPort != null)
            {
                // Log($"==[[{_ray?.ToString() ?? "null"}]]==> {TargetPort.Parent}");
                TargetPort.Ray = _ray;
            }
        }

        public virtual bool ConnectableTo([NotNull] PortBase targetPort)
        {
            if (targetPort == this) return false;
            var tt = targetPort.RayType;
            var mt = RayType;
            if (mt == tt) return true;
            if (tt == typeof(SignalRay)) return true;
            if (mt.IsSubclassOf(tt)) return true;
            return false;
        }

        /// <summary>
        /// Will auto disconnect the Source port of targetPort
        /// </summary>
        /// <param name="targetPort"></param>
        public void ConnectTo([NotNull] PortBase targetPort)
        {
            if(targetPort is PortBase tp)
            {
                if (TargetPort != tp && ConnectableTo(targetPort))
                {
                    if (tp.SourcePort is PortBase tpsp)
                    {
                        tpsp.TargetPort = null;
                    }
                    tp.SourcePort = this;
                    if (TargetPort is PortBase atp)
                    {
                        atp.SourcePort = null;
                    }
                    TargetPort = targetPort;
                }
                else
                {
                    Log($"Failed to connect: {targetPort}");
                }
            }
        }


        public virtual void ClearConnections()
        {
            if(TargetPort is PortBase pbt)
            {
                pbt.SourcePort = null;
                TargetPort = null;
            }

            if(SourcePort is PortBase pbf)
            {
                pbf.TargetPort = null;
                SourcePort = null;
            }
        }

        public abstract PortBase ClonePort();

    }
}
