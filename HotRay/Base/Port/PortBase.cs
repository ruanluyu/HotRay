using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
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

        IRay? _ray;
        public virtual IRay? Ray
        {
            set
            {
                _ray = value;
                if (_targetPorts != null)
                {
                    if(_ray == null)
                    {
                        for (int i = 0; i < _targetPorts.Count; i++)
                        {
                            _targetPorts[i].Ray = null;
                        }
                    }
                    else
                    {
                        for (int i = 0; i < _targetPorts.Count; i++)
                        {
                            _targetPorts[i].Ray = i == 0 ? _ray : _ray.RayClone();
                        }
                    }
                }
            }
            get
            {
                if (_ray == null)
                {
                    if(SourcePort != null && SourcePort.Ray != null) // supports some dynamic connection in runtime. 
                    {
                        _ray = SourcePort.Ray.RayClone();
                    }
                }
                return _ray;
            }
        }
        

        public abstract bool ConnectableTo([NotNull] IPort targetPort);

        public void ConnectTo([NotNull] IPort targetPort)
        {
            if (ConnectableTo(targetPort) && !_TargetPorts.Contains(targetPort))
            {
                _TargetPorts.Add(targetPort);
                if(targetPort.SourcePort != null)
                    targetPort.SourcePort.DisconnectTo(targetPort);
                targetPort.SourcePort = this;
            }
        }

        public void DisconnectTo([NotNull] IPort targetPort)
        {
            targetPort.SourcePort = null;
            _TargetPorts.Remove(targetPort);
        }

        public virtual void ClearConnections()
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
