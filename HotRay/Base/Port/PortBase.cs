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
        }

        RayBase? _ray;
        bool _changed;


        /// <summary>
        /// It returns _changed value then resets it to false. 
        /// </summary>
        public bool ChangedSinceLastCheck{
            get
            {
                if (_changed)
                {
                    ResetChanged();
                    return true;
                }
                return false;
            }
        }


        public bool Changed => _changed;

        public void ResetChanged() => _changed = false;


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


        public abstract PortBase ClonePort();

    }
}
