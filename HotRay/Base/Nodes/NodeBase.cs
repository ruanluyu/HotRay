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

        protected static readonly PortBase[] SharedEmptyPorts = new PortBase[0];


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

        public virtual void Init()
        {
            InitInPorts();
            InitOutPorts();
        }

        public virtual IEnumerator<Status> GetRoutine()
        {
            yield break;
        }

        public abstract IReadOnlyList<PortBase> InPorts
        {
            get;
        }


        public abstract IReadOnlyList<PortBase> OutPorts
        {
            get;
        }


        private static PortBase? _GetPortByIndex(IReadOnlyList<PortBase> list, int id)
        {
            if (id < 0 || id >= list.Count) return null;
            return list[id];
        }
        private static PortBase? _GetPortByUID(IReadOnlyList<PortBase> list, UIDType uid)
        {
            return list.FirstOrDefault(p => p.UID == uid);
        }

        private static IEnumerable<PortBase> _GetPortsByName(IReadOnlyList<PortBase> list, string name)
        {
            return list.Where(p => p.Name == name);
        }

        public virtual PortBase? GetInPortByIndex(int id) => _GetPortByIndex(InPorts, id);
        public virtual PortBase? GetOutPortByIndex(int id) => _GetPortByIndex(OutPorts, id);

        public virtual PortBase? GetInPortByUID(UIDType uid) => _GetPortByUID(InPorts, uid);
        public virtual PortBase? GetOutPortByUID(UIDType uid) => _GetPortByUID(OutPorts, uid);

        public virtual IEnumerable<PortBase> GetInPortsByName(string name) => _GetPortsByName(InPorts, name);
        public virtual IEnumerable<PortBase> GetOutPortsByName(string name) => _GetPortsByName(OutPorts, name);

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
    }
}
