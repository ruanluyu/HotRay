using HotRay.Base.Port;
using HotRay.Base.Ray;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using static HotRay.Base.Nodes.INode;

namespace HotRay.Base.Nodes
{
    public abstract class NodeBase: BaseObject, INode
    {

        protected static readonly IPort[] sharedEmptyPorts = new IPort[0];

        protected Port<rayT> CreatePort<rayT>() where rayT : RayBase
        {
            var p = new Port<rayT>();
            p.Parent = this;
            return p;
        }

        protected Port<rayT> CreatePort<rayT>(Port<rayT> other) where rayT : RayBase
        {
            var p = new Port<rayT>(other);
            p.Parent = this;
            return p;
        }



        public NodeBase() : base()
        {
            
        }

        public NodeBase(NodeBase other): base(other)
        {

        }

        public virtual void Init()
        {
            InitInputPorts();
            InitOutputPorts();
        }

        public virtual IEnumerator<Status> GetRoutine()
        {
            yield break;
        }

        public abstract IPort[] InputPorts
        {
            get;
        }
        public abstract IPort[] OutputPorts
        {
            get;
        }

        public virtual void InitInputPorts()
        {
            var p = InputPorts;
            if (p == null) return;
            for (int i = 0; i < p.Length; i++)
            {
                p[i].Ray = null;
            }
        }

        public virtual void InitOutputPorts()
        {
            var p = OutputPorts;
            if (p == null) return;
            for (int i = 0; i < p.Length; i++)
            {
                p[i].Ray = null;
            }
        }

        
    }
}
