using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotRay.Base.Port
{
    public abstract class PortBase : BaseObject, IPort
    {
        public PortBase() { }
        public PortBase(PortBase other) : base(other) 
        {
            if(other.SourcePort != null)
            {
                other.SourcePort.ConnectTo(this);
            }
        }

        public virtual RayBase? Ray
        {
            get; set;
        }

        List<IPort>? _targetPorts;
        List<IPort> _TargetPorts
        {
            get
            {
                if (_targetPorts == null) _targetPorts = new List<IPort>();
                return _targetPorts;
            }
        }

        public IPort? SourcePort { get; set; }

        public IReadOnlyList<IPort> TargetPorts
        {
            get
            {
                return _TargetPorts;
            }
        }

        public abstract bool ConnectableTo(IPort? targetPort);

        public void ConnectTo(IPort? targetPort)
        {
            if (targetPort == null) return;
            if(ConnectableTo(targetPort) && !_TargetPorts.Contains(targetPort))
            {
                _TargetPorts.Add(targetPort);
                if(targetPort.SourcePort != null)
                    targetPort.SourcePort.DisconnectTo(targetPort);
                targetPort.SourcePort = this;
            }
        }

        public void DisconnectTo(IPort? targetPort)
        {
            if (targetPort == null) return;
            targetPort.SourcePort = null;
            _TargetPorts.Remove(targetPort);
        }

        public abstract void SendRay();
        public virtual void ClearConnection()
        {
            foreach (var p in _TargetPorts.ToArray())
            {
                DisconnectTo(p);
            }
            if (SourcePort != null) SourcePort.DisconnectTo(this);
            SourcePort = null;
        }
    }
}
