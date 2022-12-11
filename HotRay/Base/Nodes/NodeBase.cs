using HotRay.Base.Nodes.Components.Containers;
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

        protected static readonly IPort[] SharedEmptyPorts = new IPort[0];


        protected Port<rayT> CreatePort<rayT>() where rayT : IRay
        {
            var p = new Port<rayT>();
            p.Parent = this;
            return p;
        }

        protected Port<rayT> CreatePortFrom<rayT>(Port<rayT> other) where rayT : IRay
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

        public BaseObject BaseObject
        {
            get
            {
                return this;
            }
        }

        public virtual void Init()
        {
            InitInPorts();
            InitOutPorts();
        }


        public abstract IReadOnlyList<IPort> InPorts
        {
            get;
        }


        public abstract IReadOnlyList<IPort> OutPorts
        {
            get;
        }


        protected void RunRoutine(IEnumerator<Status> routine)
        {
            if(Parent is Box b)
            {
                b.RegisterRoutine(this, routine);
            }
        }


        private static IPort? _GetPortByIndex(IReadOnlyList<IPort> list, int id)
        {
            if (id < 0 || id >= list.Count) return null;
            return list[id];
        }
        private static IPort? _GetPortByUID(IReadOnlyList<IPort> list, UIDType uid)
        {
            return list.FirstOrDefault(p => p.BaseObject.UID == uid);
        }

        private static IEnumerable<IPort> _GetPortsByName(IReadOnlyList<IPort> list, string name)
        {
            return list.Where(p => p.BaseObject.Name == name);
        }

        public virtual IPort? GetInPortByIndex(int id) => _GetPortByIndex(InPorts, id);
        public virtual IPort? GetOutPortByIndex(int id) => _GetPortByIndex(OutPorts, id);

        public virtual IPort? GetInPortByUID(UIDType uid) => _GetPortByUID(InPorts, uid);
        public virtual IPort? GetOutPortByUID(UIDType uid) => _GetPortByUID(OutPorts, uid);

        public virtual IEnumerable<IPort> GetInPortsByName(string name) => _GetPortsByName(InPorts, name);
        public virtual IEnumerable<IPort> GetOutPortsByName(string name) => _GetPortsByName(OutPorts, name);

        public virtual void ClearInConnections()
        {
            foreach (var p in InPorts)
            {
                p.ClearConnections();
            }
        }
        public virtual void ClearOutConnections()
        {
            foreach (var p in OutPorts)
            {
                p.ClearConnections();
            }
        }

        public virtual void InitInPorts()
        {
            foreach (var p in InPorts)
            {
                p.Ray = null;
            }
        }

        public virtual void InitOutPorts()
        {
            foreach (var p in OutPorts)
            {
                p.Ray = null;
            }
        }

        public abstract INode CloneNode();

        public virtual void OnPortUpdate(IPort inport)
        {

        }
        
        public virtual void OnEntry()
        {

        }
    }
}
